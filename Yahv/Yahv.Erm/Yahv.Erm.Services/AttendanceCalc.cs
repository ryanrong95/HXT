using System;
using System.Data;
using System.IO;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Yahv.Erm.Services.DYJRSService;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using System.Collections.Generic;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 考勤计算
    /// </summary>
    public class AttendanceCalc
    {
        /// <summary>
        /// 考勤计算
        /// </summary>
        public void Calculate(DateTime date, AttendCalcStep steps, string enterpriseID = "DBAEAB43B47EB4299DD1D62F764E6B6A", string enterpriseID2 = "B86D16EC7F0C54EA106DC97A29250875", string staffId = "", string scheduleId = "")
        {
            try
            {
                using (var reponsitory = new PvbErmReponsitory())
                {
                    string[] enterpriseIds = new string[2]
                    {
                        enterpriseID,enterpriseID2
                    };

                    //1、将大赢家数据同步至Logs_Attend
                    if (steps.HasFlag(AttendCalcStep.SyncLogs))
                    {
                        SyncData(reponsitory, date.Date, enterpriseIds: enterpriseIds);
                    }

                    //2、根据Logs_Attend初始化Pasts_Attend
                    if (steps.HasFlag(AttendCalcStep.InitPasts))
                    {
                        InitAttend(reponsitory, date.Date, enterpriseIds: enterpriseIds);
                    }

                    //3、根据Logs_Attend打卡时间更新Pasts_Attend打卡时间
                    if (steps.HasFlag(AttendCalcStep.ModifyPastsTime))
                    {
                        ModifyAttendTime(reponsitory, date.Date);
                    }

                    //4、根据Pasts_Attend里的打卡时间，更新考勤状态
                    if (steps.HasFlag(AttendCalcStep.ModifyPastsStatus))
                    {
                        ModifyAttendStatus(reponsitory, date.Date, staffId: staffId, scheduleId: scheduleId, enterpriseIds: enterpriseIds);
                    }

                    //5、根据个人日程重新更新考勤状态
                    if (steps.HasFlag(AttendCalcStep.ModifyPastsStatusBySched))
                    {
                        ModifyAttendStatusBySchedPrivate(reponsitory, date, staffId: staffId, enterpriseIds: enterpriseIds);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 功能函数
        /// <summary>
        /// 每天凌晨初始化当天考勤记录
        /// </summary>
        void InitAttend(PvbErmReponsitory reponsitory, DateTime date, string staffID = "", params string[] enterpriseIds)
        {
            //员工班别日历
            var linq = GetStaffsCalendar(reponsitory, enterpriseIds).Where(item => item.Date == date.Date);

            //考勤结果表
            var pastsStaffIds = new PastsAttendRoll(reponsitory).Where(item => item.Date == date.Date).Select(item => item.StaffID).ToArray();

            //获取没有初始化考勤的记录
            var array = linq.Where(item => !pastsStaffIds.Contains(item.StaffID)).ToArray();

            if (!string.IsNullOrWhiteSpace(staffID))
            {
                array = array.Where(item => item.StaffID == staffID).ToArray();
            }

            //数据表
            DataTable dt = GetPastsAttend();
            DataRow dr;
            DateTime now = DateTime.Now;


            var amOrPms = ExtendsEnum.ToArray<AmOrPm>();

            foreach (var arr in array)
            {
                foreach (var amOrPm in amOrPms)
                {
                    dr = dt.NewRow();

                    dr["ID"] = arr.Date.ToString("yyyyMMdd") + amOrPm + arr.StaffID;
                    dr["Date"] = arr.Date;
                    dr["AmOrPm"] = amOrPm;
                    dr["StaffID"] = arr.StaffID;
                    dr["SchedulingID"] = arr.SchedulingID;
                    dr["CreateDate"] = now;
                    dr["ModifyDate"] = now;
                    dr["InFact"] = (int)(arr.Method == ScheduleMethod.Work ? AttendInFactType.Absenteeism :
                        (arr.Method == ScheduleMethod.PublicHoliday ? AttendInFactType.PublicHoliday :
                        (arr.Method == ScheduleMethod.LegalHoliday ? AttendInFactType.LegalHolidays : AttendInFactType.Absenteeism)));
                    dr["IsLater"] = false;
                    dr["IsEarly"] = false;
                    dr["OnWorkRemedy"] = false;
                    dr["OffWorkRemedy"] = false;
                    dt.Rows.Add(dr);
                }
            }
            if (dt.Rows.Count > 0)
            {
                reponsitory.SqlBulkCopyByDatatable("Pasts_Attend", dt);
            }
        }

        /// <summary>
        /// 同步大赢家数据
        /// </summary>
        void SyncData(PvbErmReponsitory reponsitory, DateTime date, params string[] enterpriseIds)
        {
            using (DYJRSService.RSServerClient service = new RSServerClient())
            {
                //芯达通员工
                var staffs = (from staff in new StaffsRoll(reponsitory)
                              where enterpriseIds.Contains(staff.EnterpriseID) && (staff.Status == StaffStatus.Period || staff.Status == StaffStatus.Normal)
                              group staff by new { staff.DyjCode, staff.ID } into groupStaff
                              select new
                              {
                                  StaffID = groupStaff.Key.ID,
                                  DyjCode = groupStaff.Key.DyjCode,
                              }).ToArray();

                //考勤记录 该部门下的
                var logsAttends = new LogsAttendRoll(reponsitory)
                    .Where(item => item.Date == date.Date && staffs.Select(s => s.StaffID).Contains(item.StaffID)).ToArray();

                string codes = string.Join(",", staffs.Where(item => !string.IsNullOrEmpty(item.DyjCode)).Select(item => item.DyjCode).Distinct());

                var result = service.GetWorkDate(codes.Trim(','), date.Date, date.Date.AddDays(1)).JsonTo<JResult<AttendDto>>();
                if (result.errCod == 0)
                {
                    DataTable dt = GetLogsAttend();
                    DataRow dr;
                    string staffID = String.Empty;

                    foreach (var attend in result.data)
                    {
                        staffID = staffs.FirstOrDefault(item => item.DyjCode == attend.UserID)?.StaffID;
                        if (string.IsNullOrWhiteSpace(staffID))
                        {
                            continue;
                        }

                        //判断是否已经插入
                        if (!logsAttends.Any(item => item.StaffID == staffID && item.CreateDate == attend.WorkBeginDate))
                        {
                            dr = dt.NewRow();
                            dr["ID"] = PKeySigner.Pick(Underly.PKeyType.AttendLog);
                            dr["Date"] = attend.WorkBeginDate.Date;
                            dr["StaffID"] = staffID;
                            dr["CreateDate"] = attend.WorkBeginDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            dr["IP"] = attend.BeginIP;
                            dt.Rows.Add(dr);
                        }

                        //如果相等，只插入一条
                        if (attend.WorkBeginDate != attend.WorkEndDate && !logsAttends.Any(item => item.StaffID == staffID && item.CreateDate == attend.WorkEndDate))
                        {
                            dr = dt.NewRow();
                            dr["ID"] = PKeySigner.Pick(Underly.PKeyType.AttendLog);
                            dr["Date"] = attend.WorkEndDate.Date;
                            dr["StaffID"] = staffID;
                            dr["CreateDate"] = attend.WorkEndDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            dr["IP"] = attend.EndIP;
                            dt.Rows.Add(dr);
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        reponsitory.SqlBulkCopyByDatatable("Logs_Attend", dt);
                    }
                }
            }
        }

        /// <summary>
        /// 更新考勤打卡时间
        /// </summary>
        void ModifyAttendTime(PvbErmReponsitory reponsitory, DateTime date)
        {
            var logsAttend = new LogsAttendRoll(reponsitory).Where(item => item.Date == date.Date).ToArray();       //打卡记录
            var schedulings = new SchedulingsOrigin(reponsitory).ToArray(); //班别
            var staffs = new StaffsRoll(reponsitory)
                .Where(item => item.Status == StaffStatus.Period || item.Status == StaffStatus.Normal)
                .Where(item => logsAttend.Select(t => t.StaffID).Contains(item.ID)).ToArray(); //考勤的员工信息
            var pastsAttend = new PastsAttendRoll(reponsitory).Where(item => item.Date == date.Date); //考勤结果

            var records = (from l in logsAttend
                           join s in staffs on l.StaffID equals s.ID
                           join sche in schedulings on s.SchedulingID equals sche.ID into ScheduingsInto
                           from scheduling in ScheduingsInto.DefaultIfEmpty()
                           orderby l.StaffID
                           select new AttendRecord
                           {
                               StaffID = l.StaffID,
                               CreateDate = l.CreateDate,
                               Scheduling = scheduling,
                           }).ToArray();

            string id = string.Empty;
            AmOrPm amOrPm;
            PastsAttend pastAttend;

            if (records.Length <= 0)
            {
                return;
            }

            //循环考勤打卡记录 更新Pasts_Attend打卡时间
            foreach (var record in records)
            {
                //如果实际班别为空直接continue
                if (record.Scheduling == null)
                {
                    continue;
                }

                //判断打卡时间 属于上午还是下午（不在上午范围的都算下午）
                amOrPm = GetAmOrPm(record.CreateDate, record.Scheduling);
                id = record.CreateDate.ToString("yyyyMMdd") + amOrPm + record.StaffID;
                pastAttend = pastsAttend.SingleOrDefault(item => item.ID == id);
                if (pastAttend == null)
                {
                    continue;
                }

                //大赢家提供的数据，只有最早和最晚的数据。不存在早上或晚上多次打卡的情况
                //上午    记录开始时间
                if (amOrPm == AmOrPm.Am)
                {
                    //更新上午开始时间
                    if (pastAttend.StartTime == null)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                            new { StartTime = record.CreateDate }, item => item.ID == id);
                    }
                    //如果上午有开始时间了，判断两个时间早晚，更新开始时间、结束时间
                    else
                    {
                        if (record.CreateDate > pastAttend.StartTime)
                        {
                            //更新上午 结束时间
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { EndTime = record.CreateDate }, item => item.ID == id);
                        }

                        if (record.CreateDate < pastAttend.StartTime)
                        {
                            //调换上午 开始时间、结束时间
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { StartTime = record.CreateDate }, item => item.ID == id);

                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { EndTime = pastAttend.StartTime }, item => item.ID == id);
                        }
                    }
                }
                //下午    记录到结束时间
                else
                {
                    if (pastAttend.EndTime == null)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(new { EndTime = record.CreateDate },
                            item => item.ID == id);
                    }
                    else
                    {
                        if (record.CreateDate < pastAttend.EndTime)
                        {
                            //更新下午午开始时间
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { StartTime = record.CreateDate }, item => item.ID == id);
                        }

                        if (record.CreateDate > pastAttend.EndTime)
                        {
                            //调换下午开始时间，结束时间
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { EndTime = record.CreateDate }, item => item.ID == id);

                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { StartTime = pastAttend.EndTime }, item => item.ID == id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据打卡记录更新考勤状态(更新前一天的)
        /// </summary>
        void ModifyAttendStatus(PvbErmReponsitory reponsitory, DateTime date, string staffId = "", string scheduleId = "", params string[] enterpriseIds)
        {
            if (enterpriseIds == null) throw new ArgumentNullException(nameof(enterpriseIds));
            var logsAttend = new LogsAttendRoll(reponsitory).Where(item => item.Date == date.Date).ToArray();       //原始打卡记录
            var schedulings = new SchedulingsOrigin(reponsitory).ToArray();     //班别
            var staffs = new StaffsRoll(reponsitory).Where(item => logsAttend.Select(l => l.StaffID).Contains(item.ID)).ToArray();        //考勤的员工信息
            var pastsArray = new PastsAttendRoll(reponsitory).Where(item => item.Date == date.Date).ToArray();      //考勤结果

            //员工ID
            if (!string.IsNullOrWhiteSpace(staffId))
            {
                staffs = staffs.Where(item => item.ID == staffId).ToArray();
            }

            //更新考勤统计结果
            PastsAttend pastsAm;        //上午
            PastsAttend pastsPm;        //下午
            Scheduling sched;      //打卡班别
            StaffCalendar staffCalendar;        //员工日历信息

            var staffsCalendar = GetStaffsCalendarAndScheduling(reponsitory, date.Date, enterpriseIds).ToArray();        //员工班别日期
            var schedulePrivate = new SchedulesPrivateOrigin(reponsitory).Where(item => item.Date == date.Date).ToArray();     //个人日程安排

            TimeSpan tsStart;     //打卡开始时间
            TimeSpan tsEnd;     //打卡结束时间
            TimeSpan tsDomain;      //阈值


            foreach (var staff in staffs)
            {
                staffCalendar = staffsCalendar.FirstOrDefault(item => item.StaffID == staff.ID);

                if (staffCalendar==null)
                {
                    continue;
                }

                //法定节假日
                if (staffCalendar.Method == ScheduleMethod.LegalHoliday)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                   new { InFact = AttendInFactType.LegalHolidays }, item => item.StaffID == staff.ID && item.Date == date.Date);
                    continue;
                }

                //公休日
                if (staffCalendar.Method == ScheduleMethod.PublicHoliday)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                  new { InFact = AttendInFactType.PublicHoliday }, item => item.StaffID == staff.ID && item.Date == date.Date);
                    continue;
                }


                //只有员工在这个日期、班别下属于工作日的时候，才更新状态。（法定节假日，公休日不修改，节假日数据初始化默认为正常）
                if (staffCalendar?.Method == ScheduleMethod.Work)
                {
                    //当staffid和scheduleId都不为空时班别使用参数传值，否则使用员工当前的班别
                    sched = !string.IsNullOrEmpty(staffId) && !string.IsNullOrEmpty(scheduleId) ? schedulings.FirstOrDefault(item => item.ID == scheduleId) : schedulings.FirstOrDefault(item => item.ID == staffCalendar.SchedulingID);
                    //sched = schedulings.FirstOrDefault(item => item.ID == staffCalendar.SchedulingID);
                    pastsAm = pastsArray.FirstOrDefault(item => item.StaffID == staff.ID && item.AmOrPm == AmOrPm.Am.ToString());
                    pastsPm = pastsArray.FirstOrDefault(item => item.StaffID == staff.ID && item.AmOrPm == AmOrPm.Pm.ToString());
                    tsDomain = new TimeSpan(0, sched.DomainValue, 0);       //阈值

                    #region 上午
                    //上午(不包含个人日程安排、并且不是系统授权)
                    if (!schedulePrivate.Any(item => item.AmOrPm == AmOrPm.Am.ToString() && item.StaffID == staff.ID)
                        && pastsAm?.InFact != AttendInFactType.SystemAuthorizing)
                    {
                        //如果下午有结束时间
                        if (pastsPm.EndTime != null)
                        {
                            //如果考勤打卡开始时间为null，上午可以不打卡
                            if (sched.AmStartTime == null)
                            {
                                //正常
                                reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                    new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                            }
                            else
                            {
                                //上午打卡时间不为空
                                if (pastsAm.StartTime != null)
                                {
                                    tsStart = (TimeSpan)pastsAm.StartTime?.TimeOfDay;
                                    //打卡开始时间小于等于上班时间
                                    if (tsStart <= sched.AmStartTime)
                                    {
                                        //正常
                                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                            new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                                    }

                                    //打卡开始时间大于上班时间，小于等于上班时间+阈值。属于迟到
                                    if (tsStart > sched.AmStartTime &&
                                        tsStart <= sched.AmStartTime?.Add(tsDomain))
                                    {
                                        //迟到
                                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                            new
                                            {
                                                InFact = AttendInFactType.Normal,
                                                IsLater = true,
                                            }, item => item.ID == pastsAm.ID);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //下午没有打卡时间，判断是否早退
                            if (pastsAm.EndTime != null && pastsAm.StartTime != null)
                            {
                                tsStart = (TimeSpan)pastsAm.StartTime?.TimeOfDay;
                                tsEnd = (TimeSpan)pastsAm.EndTime?.TimeOfDay;

                                //打卡开始时间小于上班开始时间 打卡结束时间大于等于上班结束时间
                                if (tsStart <= sched.AmStartTime && tsEnd >= sched.AmEndTime)
                                {
                                    //正常
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                                }


                                //打卡开始时间大于上班时间，小于等于上班时间+阈值。属于迟到
                                if (tsStart > sched.AmStartTime &&
                                    tsStart <= sched.AmStartTime?.Add(tsDomain))
                                {
                                    //迟到
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsLater = true,
                                        }, item => item.ID == pastsAm.ID);
                                }


                                //打卡结束时间大于等于下班时间-阈值，小于上午下班时间
                                if (tsEnd >= sched.AmEndTime?.Add(-tsDomain) && tsEnd < sched.AmEndTime)
                                {
                                    //早退
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsEarly = true,
                                        }, item => item.ID == pastsAm.ID);
                                }
                            }

                        }
                    }
                    #endregion

                    #region 下午
                    //下午(没有个人安排 并且 不是系统授权)
                    if (!schedulePrivate.Any(item => item.AmOrPm == AmOrPm.Pm.ToString() && item.StaffID == staff.ID)
                         && pastsPm?.InFact != AttendInFactType.SystemAuthorizing)
                    {
                        //如果上午有打卡记录，或者该班别考勤上午不需要打卡
                        if (pastsAm.StartTime != null || sched.AmStartTime == null)
                        {
                            if (pastsPm.EndTime != null)
                            {
                                tsStart = (TimeSpan)pastsPm.EndTime?.TimeOfDay;

                                //下班打卡时间 大于等于（下班时间-阈值）
                                if (tsStart >= sched.PmEndTime.Add(-tsDomain) &&
                                    tsStart < sched.PmEndTime)
                                {
                                    //早退
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsEarly = true,
                                        }, item => item.ID == pastsPm.ID);
                                }

                                //打卡时间大于等于下班时间
                                if (tsStart >= sched.PmEndTime)
                                {
                                    //正常
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                        }, item => item.ID == pastsPm.ID);
                                }
                            }
                        }
                        else
                        {
                            //下午上班时间和下班时间不能为空
                            if (pastsPm.StartTime != null && pastsPm.EndTime != null)
                            {
                                tsStart = (TimeSpan)pastsPm.StartTime?.TimeOfDay;
                                tsEnd = (TimeSpan)pastsPm.EndTime?.TimeOfDay;

                                //早退
                                if (tsEnd >= sched.PmEndTime.Add(-tsDomain) && tsEnd < sched.PmEndTime)
                                {
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsEarly = true,
                                        }, item => item.ID == pastsPm.ID);
                                }

                                //正常
                                if (tsStart <= sched.PmStartTime && tsEnd >= sched.PmEndTime)
                                {
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal
                                        }, item => item.ID == pastsPm.ID);
                                }

                                //迟到
                                if (tsStart > sched.PmStartTime && tsStart <= sched.PmStartTime.Add(tsDomain))
                                {
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsLater = true,
                                        }, item => item.ID == pastsPm.ID);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 根据个人日程安排更新考勤状态
        /// </summary>
        void ModifyAttendStatusBySchedPrivate(PvbErmReponsitory reponsitory, DateTime date, string staffId = "", params string[] enterpriseIds)
        {
            //获取个人日程安排
            var schedPrivate = new SchedulesPrivateOrigin(reponsitory).Where(item => item.CreateDate >= date).ToArray();

            if (!string.IsNullOrWhiteSpace(staffId))
            {
                schedPrivate = schedPrivate.Where(item => item.StaffID == staffId).ToArray();
            }

            //遍历循环个人日程
            foreach (var sched in schedPrivate)
            {
                Recalculate(reponsitory, sched.StaffID, sched.Date, enterpriseIds);
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 考勤结果
        /// </summary>
        /// <returns></returns>
        private DataTable GetPastsAttend()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Date");
            dt.Columns.Add("AmOrPm");
            dt.Columns.Add("StaffID");
            dt.Columns.Add("SchedulingID");
            dt.Columns.Add("CreateDate");
            dt.Columns.Add("ModifyDate");
            dt.Columns.Add("InFact");
            dt.Columns.Add("IsLater");
            dt.Columns.Add("IsEarly");
            dt.Columns.Add("OnWorkRemedy");
            dt.Columns.Add("OffWorkRemedy");

            return dt;
        }

        /// <summary>
        /// 原始考勤打卡记录
        /// </summary>
        /// <returns></returns>
        private DataTable GetLogsAttend()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Date");
            dt.Columns.Add("StaffID");
            dt.Columns.Add("CreateDate");
            dt.Columns.Add("IP");

            return dt;
        }

        /// <summary>
        /// 获取上午or下午
        /// </summary>
        /// <returns></returns>
        private AmOrPm GetAmOrPm(DateTime dateTime, Scheduling scheduling)
        {
            //上午开始时间    如果null，默认为00:00:00
            TimeSpan amBegin = scheduling.AmStartTime ?? new TimeSpan(0, 0, 0);
            //上午结束时间    如果null，默认为12:00:00
            TimeSpan amEnd = scheduling.AmEndTime ?? new TimeSpan(12, 0, 0);

            TimeSpan time = dateTime.TimeOfDay;

            if (time <= amEnd)
            {
                return AmOrPm.Am;
            }
            else
            {
                return AmOrPm.Pm;
            }
        }

        /// <summary>
        /// 返回考勤结果
        /// </summary>
        private AttendInFactType GetAttendInFact(AmOrPm amOrPm, PastsAttend amAttend, PastsAttend pmAttend, Scheduling scheduling)
        {
            AttendInFactType result = AttendInFactType.Absenteeism;

            var start = amAttend.StartTime;
            var end = pmAttend.EndTime;

            var am_start = scheduling.AmStartTime;      //班别上午开始时间
            var am_end = scheduling.AmStartTime;        //班别上午结束时间
            var pm_start = scheduling.PmStartTime;      //班别下午开始时间
            var pm_end = scheduling.PmEndTime;          //班别下午结束时间

            if (amOrPm == AmOrPm.Am)
            {
                if (amAttend.StartTime == null)
                {

                }
            }
            else
            {

            }

            return result;
        }

        /// <summary>
        /// 获取员工日历（班别拿的是员工当前班别）
        /// </summary>
        /// <returns></returns>
        private IQueryable<StaffCalendar> GetStaffsCalendar(PvbErmReponsitory reponsitory, params string[] enterpriseIds)
        {
            //芯达通员工
            var staffs = new StaffsRoll(reponsitory).Where(item => enterpriseIds.Contains(item.EnterpriseID)
            && (item.Status == StaffStatus.Period || item.Status == StaffStatus.Normal));       //芯达通员工

            //日期
            var schedulesPub = new SchedulesPublicOrigin(reponsitory);

            //员工班别信息
            return from staff in staffs
                   join schdPub in schedulesPub on new { id1 = staff.SchedulingID, id2 = staff.RegionID }
                       equals new { id1 = schdPub.ShiftID, id2 = schdPub.RegionID }
                   select new StaffCalendar
                   {
                       StaffID = staff.ID,
                       ShiftID = schdPub.ShiftID,
                       SchedulingID = schdPub.SchedulingID,        //打卡时间
                       Method = schdPub.Method,
                       Date = schdPub.Date,
                   };
        }

        /// <summary>
        /// 获取员工日历（调用前必须初始化了考勤结果）
        /// </summary>
        /// <returns></returns>
        private IQueryable<StaffCalendar> GetStaffsCalendarAndScheduling(PvbErmReponsitory reponsitory, DateTime date, params string[] enterpriseIds)
        {
            //芯达通员工
            var staffs = new StaffsRoll(reponsitory).Where(item => enterpriseIds.Contains(item.EnterpriseID)
            && (item.Status == StaffStatus.Period || item.Status == StaffStatus.Normal));       //芯达通员工

            //日期
            var schedulesPub = new SchedulesPublicOrigin(reponsitory).Where(item => item.Date == date.Date);

            //员工考勤结果
            var pastsArray = new PastsAttendRoll(reponsitory).Where(item => item.Date == date.Date);

            //员工班别信息
            return from staff in staffs
                   join schdPub in schedulesPub on new { id2 = staff.RegionID }
                       equals new { id2 = schdPub.RegionID }
                   join attend in pastsArray on staff.ID equals attend.StaffID
                   select new StaffCalendar
                   {
                       StaffID = staff.ID,
                       ShiftID = attend.StaffID,
                       SchedulingID = attend.SchedulingID,        //打卡时间
                       Method = schdPub.Method,
                       Date = schdPub.Date,
                   };
        }

        /// <summary>
        /// 大赢家数据导入到临时表
        /// </summary>
        internal void InitDyjData()
        {
            string Xdt = "深圳市芯达通供应链管理有限公司".MD5();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("StaffID");
            dt.Columns.Add("DyjID");
            dt.Columns.Add("Date");
            dt.Columns.Add("StartTime");
            dt.Columns.Add("EndTime");
            dt.Columns.Add("BeginIP");
            dt.Columns.Add("EndIP");

            var adminsID = new StaffAlls()
                .Where(item => item.Labour.EnterpriseID == Xdt)
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).Select(item => item.AdminID).ToArray();
            var staffsID = new AdminsRoll().Where(item => adminsID.Contains(item.ID)).Select(item => item.StaffID).ToArray();

            var staffs = new StaffsRoll().Where(item => staffsID.Contains(item.ID)).ToArray();

            var uids = string.Join(",", new StaffsRoll().Where(item => staffsID.Contains(item.ID))
                .Select(item => item.DyjCode).Distinct()).Replace(",,", ",").Trim(',');

            using (var service = new RSServerClient())
            using (var reponsitory = new PvbErmReponsitory())
            {
                DateTime dateTime = Convert.ToDateTime("2019-01-01");
                DataRow dr;
                Yahv.Erm.Services.Models.Origins.Staff staff = null;
                DateTime date;

                for (int i = 1; i < 17; i++)
                {
                    var data = service.GetWorkDate(uids, dateTime, dateTime.AddMonths(1)).JsonTo<JResult<AttendDto>>();

                    if (data.errCod == 0)
                    {
                        foreach (var dto in data.data)
                        {
                            dr = dt.NewRow();
                            staff = staffs.FirstOrDefault(item => item.DyjCode == dto.UserID);

                            dr["ID"] = dto.WorkBeginDate.ToString("yyyyMMdd") + staff.ID;
                            dr["StaffID"] = staff.ID;
                            dr["DyjID"] = dto.UserID;
                            dr["Date"] = dto.WorkBeginDate.Date;

                            dr["StartTime"] = dto.WorkBeginDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            dr["EndTime"] = dto.WorkEndDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            dr["BeginIP"] = dto.BeginIP;
                            dr["EndIP"] = dto.EndIP;

                            dt.Rows.Add(dr);
                        }
                    }


                    dateTime = dateTime.AddMonths(1);
                }

                if (dt.Rows.Count > 0)
                {
                    reponsitory.SqlBulkCopyByDatatable("Temp_DyjAttend", dt);
                }
            }
        }

        /// <summary>
        /// 个人日程类型转考勤结果类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private AttendInFactType ConvertAttendInFactType(SchedulePrivateType type)
        {
            AttendInFactType result = AttendInFactType.Normal;

            switch (type)
            {
                //公务
                case SchedulePrivateType.OfficialBusiness:
                    result = AttendInFactType.OfficialBusiness;
                    break;
                //公差
                case SchedulePrivateType.BusinessTrip:
                    result = AttendInFactType.BusinessTrip;
                    break;
                //年假
                case SchedulePrivateType.AnnualLeave:
                //调休
                case SchedulePrivateType.LeaveInLieu:
                //婚假
                case SchedulePrivateType.MarriageLeave:
                //产检假
                case SchedulePrivateType.ProductionInspectionLeave:
                //陪产假
                case SchedulePrivateType.PaternityLeave:
                //产假
                case SchedulePrivateType.MaternityLeave:
                //丧假
                case SchedulePrivateType.FuneralLeave:
                    result = AttendInFactType.PaidLeave;
                    break;
                //事假
                case SchedulePrivateType.CasualLeave:
                    result = AttendInFactType.CasualLeave;
                    break;
                //病假
                case SchedulePrivateType.SickLeave:
                    result = AttendInFactType.SickLeave;
                    break;
                //加班
                case SchedulePrivateType.Overtime:
                    result = AttendInFactType.Overtime;
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 重新计算考勤结果
        /// </summary>
        /// <remarks>已经同步过大赢家考勤，已经初始化过考勤数据</remarks>
        /// <param name="staffID">员工ID</param>
        /// <param name="date">日期</param>
        private void Recalculate(PvbErmReponsitory reponsitory, string staffID, DateTime date, params string[] enterpriseIds)
        {
            var staffCalendar = GetStaffsCalendarAndScheduling(reponsitory, date, enterpriseIds).FirstOrDefault(item => item.StaffID == staffID);        //员工班别日期
            var pasts = new PastsAttendRoll(reponsitory).Where(item => item.Date == date && item.StaffID == staffID);
            var schedPrivate = new SchedulesPrivateOrigin(reponsitory).Where(item => item.StaffID == staffID && item.Date == date);

            //事前请假，提前生成考勤记录
            if (pasts == null || !pasts.Any())
            {
                InitAttend(reponsitory, date, staffID: staffID, enterpriseIds: enterpriseIds);
                pasts = new PastsAttendRoll(reponsitory).Where(item => item.Date == date && item.StaffID == staffID);
            }

            if (staffCalendar == null)
            {
                return;
            }

            //法定节假日 公休日
            //如果有个人日程，按照个人日程的状态赋值
            if (staffCalendar.Method == ScheduleMethod.LegalHoliday || staffCalendar.Method == ScheduleMethod.PublicHoliday)
            {
                if (schedPrivate != null && schedPrivate.Any(item => item.AmOrPm == AmOrPm.Am.ToString()))
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                           new
                           {
                               InFact = ConvertAttendInFactType(schedPrivate.FirstOrDefault(item => item.AmOrPm == AmOrPm.Am.ToString()).SchedulePrivateType)

                           }, item => item.StaffID == staffID && item.Date == date);
                }

                if (schedPrivate != null && schedPrivate.Any(item => item.AmOrPm == AmOrPm.Pm.ToString()))
                {

                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                           new
                           {
                               InFact = ConvertAttendInFactType(schedPrivate.FirstOrDefault(item => item.AmOrPm == AmOrPm.Pm.ToString()).SchedulePrivateType)

                           }, item => item.StaffID == staffID && item.Date == date);
                }

                return;
            }



            //根据个人日程安排 更新考勤状态
            if (schedPrivate != null && schedPrivate.Any())
            {
                foreach (var sched in schedPrivate)
                {
                    //如果是补签
                    if (sched.SchedulePrivateType == SchedulePrivateType.ReSign)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                   new
                                   {
                                       InFact = AttendInFactType.Normal,
                                       OffWorkRemedy = sched.OffWorkRemedy,
                                       OnWorkRemedy = sched.OnWorkRemedy,
                                   }
                                   , item => item.StaffID == sched.StaffID
                                   && item.Date == date
                                   && item.AmOrPm == sched.AmOrPm);
                    }
                    else
                    {
                        var type = ConvertAttendInFactType(sched.SchedulePrivateType);
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                   new { InFact = type }
                                   , item => item.StaffID == sched.StaffID
                                   && item.Date == date
                                   && item.AmOrPm == sched.AmOrPm);
                    }
                }
            }


            //根据考勤原始打卡时间，个人日程安排，重新计算当天考勤结果
            var scheduling = new SchedulingsOrigin(reponsitory).FirstOrDefault(item => item.ID == staffCalendar.SchedulingID);      //班别
            var pastsAm = pasts.FirstOrDefault(item => item.AmOrPm == AmOrPm.Am.ToString());
            var pastsPm = pasts.FirstOrDefault(item => item.AmOrPm == AmOrPm.Pm.ToString());
            var tsDomain = new TimeSpan(0, scheduling.DomainValue, 0);       //阈值
            TimeSpan tsStart;     //打卡开始时间
            TimeSpan tsEnd;     //打卡结束时间

            #region 上午
            //如果上午是旷工，根据原始打卡时间和个人日程安排重新更新考勤状态
            if (pastsAm.InFact == AttendInFactType.Absenteeism)
            {
                //如果下午有结束时间、或者下午状态不为旷工
                if (pastsPm.EndTime != null || pastsPm.InFact != AttendInFactType.Absenteeism)
                {
                    //如果考勤打卡开始时间为null，上午可以不打卡
                    if (scheduling.AmStartTime == null)
                    {
                        //正常
                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                            new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                    }
                    else
                    {
                        //上午打卡时间不为空
                        if (pastsAm.StartTime != null)
                        {
                            tsStart = (TimeSpan)pastsAm.StartTime?.TimeOfDay;
                            //打卡开始时间小于上班时间
                            if (tsStart <= scheduling.AmStartTime)
                            {
                                //正常
                                reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                    new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                            }

                            //打卡开始时间大于上班时间，小于等于上班时间+阈值。属于迟到
                            if (tsStart > scheduling.AmStartTime &&
                                tsStart <= scheduling.AmStartTime?.Add(tsDomain))
                            {
                                //迟到
                                reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                    new
                                    {
                                        InFact = AttendInFactType.Normal,
                                        IsLater = true,
                                    }, item => item.ID == pastsAm.ID);
                            }


                            //如果下午是 事假、病假、带薪休假、公务、公差
                            //判断有没有早退
                            if (IsLeave(pastsPm.InFact))
                            {
                                if (pastsAm.EndTime != null)
                                {
                                    tsEnd = (TimeSpan)pastsAm.EndTime?.TimeOfDay;

                                    //打卡结束时间大于下班时间-阈值，小于上午下班时间
                                    if (tsEnd >= scheduling.AmEndTime?.Add(-tsDomain) && tsEnd < scheduling.AmEndTime)
                                    {
                                        //早退
                                        reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                            new
                                            {
                                                InFact = AttendInFactType.Normal,
                                                IsEarly = true,
                                            }, item => item.ID == pastsAm.ID);
                                    }

                                    //打卡结束时间大于上午下班时间  正常
                                    //if (tsStart <= scheduling.AmStartTime && tsEnd >= scheduling.AmEndTime)
                                    //{
                                    //    //正常
                                    //    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                    //       new
                                    //       {
                                    //           InFact = AttendInFactType.Normal,
                                    //           IsEarly = true,
                                    //       }, item => item.ID == pastsAm.ID);
                                    //}
                                }
                            }
                        }
                    }
                }
                else
                {
                    //下午没有打卡时间，判断是否早退
                    if (pastsAm.EndTime != null && pastsAm.StartTime != null)
                    {
                        tsStart = (TimeSpan)pastsAm.StartTime?.TimeOfDay;
                        tsEnd = (TimeSpan)pastsAm.EndTime?.TimeOfDay;

                        //打卡开始时间小于上班时间
                        if (tsStart <= scheduling.AmStartTime && tsEnd >= scheduling.AmEndTime)
                        {
                            //正常
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new { InFact = AttendInFactType.Normal }, item => item.ID == pastsAm.ID);
                        }


                        //打卡开始时间大于等于上班时间，小于等于上班时间+阈值。属于迟到
                        if (tsStart > scheduling.AmStartTime &&
                            tsStart <= scheduling.AmStartTime?.Add(tsDomain))
                        {
                            //迟到
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                    IsLater = true,
                                }, item => item.ID == pastsAm.ID);
                        }


                        //打卡结束时间大于下班时间-阈值，小于上午下班时间
                        if (tsEnd >= scheduling.AmEndTime?.Add(-tsDomain) && tsEnd < scheduling.AmEndTime)
                        {
                            //早退
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                    IsEarly = true,
                                }, item => item.ID == pastsAm.ID);
                        }
                    }

                }
            }
            #endregion

            #region 下午
            //如果下午为旷工，重新根据打卡时间和个人日程更新状态
            if (pastsPm.InFact == AttendInFactType.Absenteeism)
            {
                //下午
                //上午打卡时间不为空、或者班别上午不需要打卡、或者上午考勤正常
                if (pastsAm.StartTime != null || scheduling.AmStartTime == null || pastsAm.InFact != AttendInFactType.Absenteeism)
                {
                    if (pastsPm.EndTime != null)
                    {
                        tsEnd = (TimeSpan)pastsPm.EndTime?.TimeOfDay;

                        //下班打卡时间 大于等于（下班时间-阈值）
                        if (tsEnd >= scheduling.PmEndTime.Add(-tsDomain) &&
                            tsEnd < scheduling.PmEndTime)
                        {
                            //早退
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                    IsEarly = true,
                                }, item => item.ID == pastsPm.ID);
                        }

                        //打卡时间大于等于下班时间
                        if (tsEnd >= scheduling.PmEndTime)
                        {
                            //正常
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                }, item => item.ID == pastsPm.ID);
                        }


                        //上午是请假，考虑是否迟到
                        if (IsLeave(pastsAm.InFact))
                        {
                            if (pastsPm.StartTime != null)
                            {
                                tsStart = (TimeSpan)pastsPm.StartTime?.TimeOfDay;

                                if (tsStart > scheduling.PmStartTime && tsStart <= scheduling.PmStartTime.Add(tsDomain))
                                {
                                    reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                        new
                                        {
                                            InFact = AttendInFactType.Normal,
                                            IsLater = true,
                                        }, item => item.ID == pastsPm.ID);
                                }
                            }

                        }
                    }
                }
                else
                {
                    //下午上班时间和下班时间不能为空
                    if (pastsPm.StartTime != null && pastsPm.EndTime != null)
                    {
                        tsStart = (TimeSpan)pastsPm.StartTime?.TimeOfDay;
                        tsEnd = (TimeSpan)pastsPm.EndTime?.TimeOfDay;

                        //早退
                        if (tsEnd >= scheduling.PmEndTime.Add(-tsDomain) && tsEnd < scheduling.PmEndTime)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                    IsEarly = true,
                                }, item => item.ID == pastsPm.ID);
                        }

                        //正常
                        if (tsStart <= scheduling.PmStartTime && tsEnd >= scheduling.PmEndTime)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal
                                }, item => item.ID == pastsPm.ID);
                        }

                        //迟到
                        if (tsStart > scheduling.PmStartTime && tsStart <= scheduling.PmStartTime.Add(tsDomain))
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
                                new
                                {
                                    InFact = AttendInFactType.Normal,
                                    IsLater = true,
                                }, item => item.ID == pastsPm.ID);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 是否休假（事假、病假、带薪休假、公务、公差）
        /// </summary>
        /// <returns></returns>
        private bool IsLeave(AttendInFactType type)
        {
            if (type == AttendInFactType.CasualLeave
                || type == AttendInFactType.SickLeave
                || type == AttendInFactType.PaidLeave
                || type == AttendInFactType.OfficialBusiness
                || type == AttendInFactType.PublicHoliday
                || type == AttendInFactType.BusinessTrip)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算考勤结果
        /// </summary>
        /// <param name="attend">考勤记录</param>
        /// <param name="scheduling">班别</param>
        /// <param name="IsHave">另半天是否有记录或者个人日程安排</param>
        /// <returns></returns>
        //private PastsAttend CalcAttendStatus(PastsAttend attend, Scheduling scheduling, bool IsHave, StaffCalendar staffCalendar, TimeSpan tsDomain, SchedulePrivate schedPri)
        //{
        //    //法定节假日
        //    if (staffCalendar.Method == ScheduleMethod.LegalHoliday)
        //    {
        //        attend.InFact = AttendInFactType.LegalHolidays;
        //    }

        //    //公休日
        //    if (staffCalendar.Method == ScheduleMethod.PublicHoliday)
        //    {
        //        attend.InFact = AttendInFactType.PublicHoliday;
        //    }

        //    //如果有个人日程安排，按照个人日程安排状态为准
        //    if (schedPri != null && !string.IsNullOrWhiteSpace(schedPri.ID))
        //    {
        //        attend.InFact = ConvertAttendInFactType(schedPri.SchedulePrivateType);

        //        //如果是补签
        //        if (schedPri.SchedulePrivateType == SchedulePrivateType.ReSign)
        //        {
        //            attend.OnWorkRemedy = schedPri.OnWorkRemedy;
        //            attend.OffWorkRemedy = schedPri.OffWorkRemedy;
        //        }
        //    }
        //    else
        //    {
        //        //只有员工在这个日期、班别下属于工作日的时候，才更新状态。（法定节假日，公休日不修改，有个人日程安排的不修改）
        //        if (staffCalendar?.Method == ScheduleMethod.Work)
        //        {
        //            TimeSpan tsStart;     //打卡开始时间
        //            TimeSpan tsEnd;     //打卡结束时间

        //            //班别考勤开始时间
        //            TimeSpan? schedStart = attend.AmOrPm == AmOrPm.Am.ToString() ? scheduling.AmStartTime : scheduling.PmStartTime;

        //            //班别考勤结束时间
        //            TimeSpan? schedEnd = attend.AmOrPm == AmOrPm.Am.ToString() ? scheduling.AmEndTime : scheduling.PmEndTime;

        //            //如果另半天有数据或者有个人日程
        //            if (IsHave)
        //            {
        //                //班别开始时间为null，开始时间可以不打卡
        //                if (schedStart == null)
        //                {
        //                    attend.InFact = AttendInFactType.Normal;
        //                }
        //                else
        //                {
        //                    //打卡开始时间不为null
        //                    if (attend.StartTime != null)
        //                    {
        //                        tsStart = TimeSpan.Parse(attend.StartTime?.ToLongTimeString());

        //                        //打卡开始时间小于上班时间
        //                        if (tsStart <= schedStart)
        //                        {
        //                            attend.InFact = AttendInFactType.Normal;
        //                        }

        //                        //打卡开始时间大于等于上班时间，小于上班时间+阈值。属于迟到
        //                        if (tsStart > schedStart && tsStart <= schedStart?.Add(tsDomain))
        //                        {
        //                            //迟到
        //                            attend.InFact = AttendInFactType.Normal;
        //                            attend.IsLater = true;
        //                        }

        //                        //如果下午是 事假、病假、带薪休假、公务、公差
        //                        //判断有没有早退
        //                        //if (IsLeave(pastsPm.InFact))
        //                        //{
        //                        //    if (pastsAm.EndTime != null)
        //                        //    {
        //                        //        tsEnd = TimeSpan.Parse(pastsAm.EndTime?.ToLongTimeString());

        //                        //        //打卡结束时间大于下班时间-阈值，小于上午下班时间
        //                        //        if (tsEnd >= scheduling.AmEndTime?.Add(-tsDomain) && tsEnd < scheduling.AmEndTime)
        //                        //        {
        //                        //            //早退
        //                        //            reponsitory.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(
        //                        //                new
        //                        //                {
        //                        //                    InFact = AttendInFactType.Normal,
        //                        //                    IsEarly = true,
        //                        //                }, item => item.ID == pastsAm.ID);
        //                        //        }

        //                        //    }
        //                        //}
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //            else
        //            {

        //            }
        //        }
        //    }


        //    return attend;
        //}
        #endregion

        #region Dto
        /// <summary>
        /// 考勤信息
        /// </summary>
        class AttendDto
        {
            /// <summary>
            /// 大赢家ID
            /// </summary>
            public string UserID { get; set; }
            /// <summary>
            /// 上班打卡时间
            /// </summary>
            public DateTime WorkBeginDate { get; set; }
            /// <summary>
            /// 下班打卡时间
            /// </summary>
            public DateTime WorkEndDate { get; set; }
            /// <summary>
            /// 上班打卡考勤机
            /// </summary>
            public string BeginIP { get; set; }
            /// <summary>
            /// 下班打卡考勤机
            /// </summary>
            public string EndIP { get; set; }
        }

        /// <summary>
        /// 考勤记录
        /// </summary>
        class AttendRecord
        {
            public string StaffID { get; set; }
            public DateTime CreateDate { get; set; }
            public Scheduling Scheduling { get; set; }
        }

        /// <summary>
        /// 员工日历
        /// </summary>
        class StaffCalendar
        {
            public string StaffID { get; set; }

            /// <summary>
            /// 所属班别
            /// </summary>
            public string ShiftID { get; set; }

            /// <summary>
            /// 实际班别
            /// </summary>
            public string SchedulingID { get; set; }

            /// <summary>
            /// 工作日、公休日、法定节假日
            /// </summary>
            public ScheduleMethod Method { get; set; }

            public System.DateTime Date { get; set; }
        }

        /// <summary>
        /// 单条数据格式
        /// </summary>
        class JResult<T>
        {
            /// <summary>
            /// 错误编码
            /// </summary>
            public int errCod { get; set; }
            /// <summary>
            /// 错误信息
            /// </summary>
            public string errMsg { get; set; }
            /// <summary>
            /// 单条数据
            /// </summary>
            public IEnumerable<T> data { get; set; }
        }
        #endregion

        #region 单例
        static AttendanceCalc current;
        static object locker = new object();

        static public AttendanceCalc Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new AttendanceCalc();
                        }
                    }
                }

                return current;
            }
        }
        #endregion
    }

    /// <summary>
    /// 计算步骤
    /// </summary>
    [Flags]
    public enum AttendCalcStep
    {
        /// <summary>
        /// 1、将大赢家数据同步至Logs_Attend
        /// </summary>
        SyncLogs = 1,
        /// <summary>
        /// 2、根据Logs_Attend初始化Pasts_Attend
        /// </summary>
        InitPasts = 2,
        /// <summary>
        /// 3、根据Logs_Attend打卡时间更新Pasts_Attend打卡时间
        /// </summary>
        ModifyPastsTime = 4,
        /// <summary>
        /// 4、根据Pasts_Attend里的打卡时间，更新考勤状态
        /// </summary>
        ModifyPastsStatus = 8,
        /// <summary>
        /// 5、根据个人日程重新更新考勤状态
        /// </summary>
        ModifyPastsStatusBySched = 16,
    }
}