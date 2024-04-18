using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;
using Yahv.AttendanceData.Import.ServiceReference1;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;

namespace Yahv.AttendanceData.Import.Services
{
    /// <summary>
    /// 考勤历史数据导入
    /// </summary>
    public partial class AttendanceService2
    {
        #region 构造器

        RegexOptions options;
        Regex regex;

        public AttendanceService2()
        {
            options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            regex = new Regex(@"(.+?)\[(.+?):(.+?)-(.+?):(.+?)\]", options);
        }

        #endregion

        #region 从文件读取的考勤历史数据

        List<Models.Attendance> attendances = new List<Models.Attendance>();

        #endregion

        #region 需要保存的数据

        //申请
        List<Layers.Data.Sqls.PvbErm.Applications> applications = new List<Layers.Data.Sqls.PvbErm.Applications>();
        List<Layers.Data.Sqls.PvbErm.ApplyVoteSteps> applyVoteSteps = new List<Layers.Data.Sqls.PvbErm.ApplyVoteSteps>();
        List<Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps> logsApplyVoteSteps = new List<Layers.Data.Sqls.PvbErm.Logs_ApplyVoteSteps>();
        List<Layers.Data.Sqls.PvbErm.MapsAppStaff> mapsAppStaff = new List<Layers.Data.Sqls.PvbErm.MapsAppStaff>();

        //考勤
        List<Layers.Data.Sqls.PvbErm.Logs_Attend> logsAttend = new List<Layers.Data.Sqls.PvbErm.Logs_Attend>();
        List<Layers.Data.Sqls.PvbErm.Pasts_Attend> pastsAttend = new List<Layers.Data.Sqls.PvbErm.Pasts_Attend>();

        //日程安排
        List<Layers.Data.Sqls.PvbErm.Schedules> schedules = new List<Layers.Data.Sqls.PvbErm.Schedules>();
        List<Layers.Data.Sqls.PvbErm.SchedulesPrivate> schedulesPrivate = new List<Layers.Data.Sqls.PvbErm.SchedulesPrivate>();

        #endregion
    }

    public partial class AttendanceService2
    {
        /// <summary>
        /// 数据读取
        /// </summary>
        /// <returns></returns>
        public AttendanceService2 Read(int year, int month)
        {
            //不需要按照公司规定打卡的员工
            var specialStaffs = System.Configuration.ConfigurationManager.AppSettings["SpecialStaffs"].Split(',');

            //班别
            var schedulings = DataManager.Current.Schedulings;

            int days = DateTime.DaysInMonth(year, month); //当月天数
            DateTime beginDate = new DateTime(year, month, 1).Date; //月初
            DateTime endDate = new DateTime(year, month, days).Date; //月末
            DateTime dueDate = DateTime.Parse($"{2020}-{5}-{25}").Date;//数据导入截止日期

            //已入职的芯达通员工
            var xdtStaffs = DataManager.Current.XdtStaffs.Where(item => item.HireDate <= endDate).ToArray();

            //当月的公共日程安排
            Models.SchedulePublic[] schedulePublic = new Views.SchedulesPublicView().Where(item => item.Date >= beginDate && item.Date <= endDate).ToArray();

            foreach (var staff in xdtStaffs)
            {
                Console.WriteLine($"DyjID: {staff.DyjID} Name: {staff.Name}");

                RSServerClient client = new RSServerClient();
                //员工当月的打卡记录
                var workDates = client.GetWorkDate(staff.DyjID, beginDate, endDate).JsonTo<Models.DyjResponse<Models.DyjModel>>().data.OrderBy(item => item.WorkBeginDate);
                //员工当月的请假记录
                var qingJias = client.GetQingJiaList(staff.DyjID, beginDate, endDate).JsonTo<Models.DyjResponse<Models.QingJia>>().data.OrderBy(item => item.sdate);

                for (int i = 1; i <= days; i++)
                {
                    DateTime date = DateTime.Parse($"{year}-{month}-{i}").Date;
                    if (date < staff.HireDate)
                    {
                        continue;
                    }
                    if (date >= dueDate)
                    {
                        continue;
                    }

                    var method = schedulePublic.First(item => item.Date == date).Method;
                    var dayOfWeek = new DateTime(year, month, i).DayOfWeek;
                    var workDate = workDates.SingleOrDefault(item => item.WorkBeginDate.Date == date);
                    Models.QingJia qingjia = null;
                    if (qingJias.Count(item => item.sdate == date) > 1)
                    {
                        qingjia = qingJias.First(item => item.sdate == date);
                        qingjia.bantian = 0;
                    }
                    else
                    {
                        qingjia = qingJias.SingleOrDefault(item => item.sdate == date);
                    }

                    var attendance = new Models.Attendance()
                    {
                        StaffID = staff.ID,
                        Name = staff.Name,
                        DyjCode = staff.DyjCode,
                        RegionID = staff.RegionID,
                        Date = date,
                    };

                    //工作日
                    if (method == ScheduleMethod.Work)
                    {
                        //如果有公务、公差、请假的情况
                        if (qingjia != null)
                        {
                            var aBan = schedulings.Single(item => item.Name == "A班");
                            DateTime amStartTime = new DateTime(year, month, i, aBan.AmStartTime.Value.Hours, aBan.AmStartTime.Value.Minutes, aBan.AmStartTime.Value.Seconds);
                            DateTime amEndTime = new DateTime(year, month, i, aBan.AmEndTime.Value.Hours, aBan.AmEndTime.Value.Minutes, aBan.AmEndTime.Value.Seconds);
                            DateTime pmStartTime = new DateTime(year, month, i, aBan.PmStartTime.Hours, aBan.PmStartTime.Minutes, aBan.PmStartTime.Seconds);
                            DateTime pmEndTime = new DateTime(year, month, i, aBan.PmEndTime.Hours, aBan.PmEndTime.Minutes, aBan.PmEndTime.Seconds);

                            attendance.SchedulingID = aBan.ID;

                            switch (qingjia.bantian)
                            {
                                //整天
                                case 0:
                                    attendance.Result = qingjia.Stype;
                                    attendance.Result_Am = qingjia.Stype;
                                    attendance.Result_Pm = qingjia.Stype;
                                    break;
                                //上午
                                case 1:
                                    attendance.Result = $"{qingjia.Stype}(上午)";
                                    attendance.Result_Am = qingjia.Stype;
                                    attendance.Result_Pm = AttendanceRecord.Normal;
                                    attendance.BeginIP = workDate?.BeginIP;
                                    attendance.EndIP = workDate?.EndIP;
                                    attendance.Date_Start = workDate == null ? Utils.DateUtil.GetPmStartTime(year, month, i) :
                                                                               workDate.WorkBeginDate < pmStartTime ? workDate.WorkBeginDate : Utils.DateUtil.GetPmStartTime(year, month, i);
                                    attendance.Date_End = workDate == null ? Utils.DateUtil.GetPmEndTime(year, month, i) :
                                                                             workDate.WorkEndDate >= pmEndTime ? workDate.WorkEndDate : Utils.DateUtil.GetPmEndTime(year, month, i);
                                    attendance.Date_Pm_Start = attendance.Date_Start;
                                    attendance.Date_Pm_End = attendance.Date_End;
                                    break;
                                //下午
                                case 2:
                                    attendance.Result = $"{qingjia.Stype}(下午)";
                                    attendance.Result_Am = AttendanceRecord.Normal;
                                    attendance.Result_Pm = qingjia.Stype;
                                    attendance.BeginIP = workDate?.BeginIP;
                                    attendance.EndIP = workDate?.EndIP;
                                    attendance.Date_Start = workDate == null ? Utils.DateUtil.GetAmStartTime(year, month, i) :
                                                                              workDate.WorkBeginDate < amStartTime ? workDate.WorkBeginDate : Utils.DateUtil.GetAmStartTime(year, month, i);
                                    attendance.Date_End = workDate == null ? Utils.DateUtil.GetAmEndTime(year, month, i) :
                                                                            workDate.WorkEndDate >= amEndTime ? workDate.WorkEndDate : Utils.DateUtil.GetAmEndTime(year, month, i);
                                    attendance.Date_Am_Start = attendance.Date_Start;
                                    attendance.Date_Pm_End = attendance.Date_End;
                                    break;
                            }
                        }
                        //没有打卡记录或是不需要按规定打卡的员工，按正常考勤处理
                        else if (workDate == null || specialStaffs.Contains(staff.Name))
                        {
                            var aBan = schedulings.Single(item => item.Name == "A班");
                            DateTime amStartTime = new DateTime(year, month, i, aBan.AmStartTime.Value.Hours, aBan.AmStartTime.Value.Minutes, aBan.AmStartTime.Value.Seconds);
                            DateTime pmEndTime = new DateTime(year, month, i, aBan.PmEndTime.Hours, aBan.PmEndTime.Minutes, aBan.PmEndTime.Seconds);

                            attendance.SchedulingID = aBan.ID;
                            attendance.Result = AttendanceRecord.Normal;
                            attendance.Result_Am = AttendanceRecord.Normal;
                            attendance.Result_Pm = AttendanceRecord.Normal;
                            attendance.BeginIP = workDate?.BeginIP;
                            attendance.EndIP = workDate?.EndIP;
                            attendance.Date_Start = workDate == null ? Utils.DateUtil.GetAmStartTime(year, month, i) :
                                                                       workDate.WorkBeginDate < amStartTime ? workDate.WorkBeginDate : Utils.DateUtil.GetAmStartTime(year, month, i);
                            attendance.Date_End = workDate == null ? Utils.DateUtil.GetPmEndTime(year, month, i) :
                                                                     workDate.WorkEndDate >= pmEndTime ? workDate.WorkEndDate : Utils.DateUtil.GetPmEndTime(year, month, i);
                            attendance.Date_Am_Start = attendance.Date_Start;
                            attendance.Date_Pm_End = attendance.Date_End;
                        }
                        //有打卡记录且需要正常考勤的员工
                        else
                        {
                            //只有一次打卡记录，则认为忘记打卡，处理成补签
                            if (workDate.WorkBeginDate == workDate.WorkEndDate)
                            {
                                attendance.SchedulingID = schedulings.Single(item => item.Name == "A班").ID;
                                attendance.Result = AttendanceRecord.SS;
                                attendance.Result_Am = AttendanceRecord.SS;
                                attendance.Result_Pm = AttendanceRecord.SS;
                                attendance.BeginIP = workDate.BeginIP;
                                attendance.EndIP = workDate.EndIP;
                                attendance.Date_Start = workDate.WorkBeginDate;
                                attendance.Date_End = workDate.WorkEndDate;
                            }
                            //有两次打卡记录
                            else
                            {
                                //班别
                                Models.Scheduling myBan = null;
                                //A班
                                var aBan = schedulings.Single(item => item.Name == "A班");
                                DateTime aBanAmStartTime_Domain = new DateTime(year, month, i, aBan.AmStartTime.Value.Hours, aBan.AmStartTime.Value.Minutes + aBan.DomainValue, aBan.AmStartTime.Value.Seconds);
                                //B班
                                var bBan = schedulings.Single(item => item.Name == "B班");
                                DateTime bBanAmStartTime_Domain = new DateTime(year, month, i, bBan.AmStartTime.Value.Hours, bBan.AmStartTime.Value.Minutes + bBan.DomainValue, bBan.AmStartTime.Value.Seconds);
                                //D班
                                var dBan = schedulings.Single(item => item.Name == "D班");
                                //E班
                                var eBan = schedulings.Single(item => item.Name == "E班");

                                //是否是哺乳假期间
                                var bl = DataManager.Current.BreastfeedingLeaves.SingleOrDefault(item => item.Name == staff.Name && item.StartDate <= date && date <= item.EndDate);

                                //正常考勤的情况
                                if (workDate.WorkBeginDate < aBanAmStartTime_Domain)
                                {
                                    if (bl == null)
                                        myBan = aBan;
                                    else
                                        myBan = dBan;
                                }
                                else if (workDate.WorkBeginDate < bBanAmStartTime_Domain)
                                {
                                    if (bl == null)
                                        myBan = bBan;
                                    else
                                        myBan = eBan;
                                }

                                if (myBan != null)
                                {
                                    DateTime myBanAmStartTime = new DateTime(year, month, i, myBan.AmStartTime.Value.Hours, myBan.AmStartTime.Value.Minutes, myBan.AmStartTime.Value.Seconds);
                                    DateTime myBanAmStartTime_Domain = new DateTime(year, month, i, myBan.AmStartTime.Value.Hours, myBan.AmStartTime.Value.Minutes + myBan.DomainValue, myBan.AmStartTime.Value.Seconds);
                                    DateTime myBanPmEndTime = new DateTime(year, month, i, myBan.PmEndTime.Hours, myBan.PmEndTime.Minutes, myBan.PmEndTime.Seconds);
                                    DateTime myBanPmEndTime_Domain = new DateTime(year, month, i, myBan.PmEndTime.Hours, myBan.PmEndTime.Minutes - myBan.DomainValue, myBan.PmEndTime.Seconds);

                                    //上午正常、下午正常
                                    if (workDate.WorkBeginDate < myBanAmStartTime && workDate.WorkEndDate >= myBanPmEndTime)
                                    {
                                        attendance.Result = AttendanceRecord.Normal;
                                        attendance.Result_Am = AttendanceRecord.Normal;
                                        attendance.Result_Pm = AttendanceRecord.Normal;
                                    }
                                    //上午迟到、下午正常
                                    if (workDate.WorkBeginDate >= myBanAmStartTime && workDate.WorkBeginDate < myBanAmStartTime_Domain && workDate.WorkEndDate >= myBanPmEndTime)
                                    {
                                        attendance.Result = AttendanceRecord.BeLate;
                                        attendance.Result_Am = AttendanceRecord.BeLate;
                                        attendance.Result_Pm = AttendanceRecord.Normal;
                                    }
                                    //上午正常、下午早退
                                    if (workDate.WorkBeginDate < myBanAmStartTime && workDate.WorkEndDate >= myBanPmEndTime_Domain && workDate.WorkEndDate < myBanPmEndTime)
                                    {
                                        attendance.Result = AttendanceRecord.EarlyLeave;
                                        attendance.Result_Am = AttendanceRecord.Normal;
                                        attendance.Result_Pm = AttendanceRecord.EarlyLeave;
                                    }
                                    //上午迟到、下午早退
                                    if (workDate.WorkBeginDate >= myBanAmStartTime && workDate.WorkBeginDate < myBanAmStartTime_Domain &&
                                        workDate.WorkEndDate >= myBanPmEndTime_Domain && workDate.WorkEndDate < myBanPmEndTime)
                                    {
                                        attendance.Result = AttendanceRecord.BL_EL;
                                        attendance.Result_Am = AttendanceRecord.BeLate;
                                        attendance.Result_Pm = AttendanceRecord.EarlyLeave;
                                    }

                                    if (attendance.Result != null)
                                    {
                                        attendance.SchedulingID = myBan.ID;
                                        attendance.Date_Start = workDate.WorkBeginDate;
                                        attendance.Date_End = workDate.WorkEndDate;
                                        attendance.BeginIP = workDate.BeginIP;
                                        attendance.EndIP = workDate.EndIP;
                                        attendance.Date_Am_Start = workDate.WorkBeginDate;
                                        attendance.Date_Pm_End = workDate.WorkEndDate;
                                    }
                                }
                            }
                        }
                    }
                    //公休日
                    else if (method == ScheduleMethod.PublicHoliday)
                    {
                        //如果有打卡记录，C班
                        if (workDate != null)
                        {
                            var cBan = schedulings.Single(item => item.Name == "C班");
                            DateTime pmStartTime = new DateTime(year, month, i, cBan.PmStartTime.Hours, cBan.PmStartTime.Minutes, cBan.PmStartTime.Seconds);
                            DateTime pmEndTime = new DateTime(year, month, i, cBan.PmEndTime.Hours, cBan.PmEndTime.Minutes, cBan.PmEndTime.Seconds);

                            attendance.SchedulingID = cBan.ID;
                            attendance.Result = AttendanceRecord.Normal;
                            attendance.Result_Am = AttendanceRecord.PublicHoliday;
                            attendance.Result_Pm = AttendanceRecord.Normal;
                            attendance.BeginIP = workDate.BeginIP;
                            attendance.EndIP = workDate.EndIP;
                            attendance.Date_Start = workDate.WorkBeginDate < pmStartTime ? workDate.WorkBeginDate : Utils.DateUtil.GetPmStartTime(year, month, i);
                            attendance.Date_End = workDate.WorkEndDate >= pmEndTime ? workDate.WorkEndDate : Utils.DateUtil.GetPmEndTime(year, month, i);
                            attendance.Date_Pm_Start = attendance.Date_Start;
                            attendance.Date_Pm_End = attendance.Date_End;
                        }
                        //否则按公休日处理
                        else
                        {
                            attendance.SchedulingID = schedulings.Single(item => item.Name == "A班").ID;
                            attendance.Result = AttendanceRecord.PublicHoliday;
                            attendance.Result_Am = AttendanceRecord.PublicHoliday;
                            attendance.Result_Pm = AttendanceRecord.PublicHoliday;
                        }
                    }
                    //法定节假日
                    else
                    {
                        attendance.SchedulingID = schedulings.Single(item => item.Name == "A班").ID;
                        //如果有打卡记录，按照加班处理
                        if (workDate != null)
                        {
                            attendance.Result = AttendanceRecord.Overtime;
                            attendance.Result_Am = AttendanceRecord.Overtime;
                            attendance.Result_Pm = AttendanceRecord.Overtime;
                            attendance.BeginIP = workDate.BeginIP;
                            attendance.EndIP = workDate.EndIP;
                            attendance.Date_Start = workDate.WorkBeginDate;
                            attendance.Date_End = workDate.WorkEndDate;
                        }
                        //否则按法定节假日处理
                        else
                        {
                            attendance.Result = AttendanceRecord.LegalHolidays;
                            attendance.Result_Am = AttendanceRecord.LegalHolidays;
                            attendance.Result_Pm = AttendanceRecord.LegalHolidays;
                        }
                    }

                    //这些员工在这些日期有考勤扣款20, 所以处理成补签
                    if ((staff.Name == "杨文" && (date == DateTime.Parse($"{2019}-{8}-{23}").Date || date == DateTime.Parse($"{2019}-{11}-{12}").Date)) ||
                        (staff.Name == "郝红梅" && date == DateTime.Parse($"{2019}-{9}-{2}").Date) ||
                        (staff.Name == "邵晨华" && date == DateTime.Parse($"{2020}-{4}-{17}").Date))
                    {
                        attendance.SchedulingID = schedulings.Single(item => item.Name == "A班").ID;
                        attendance.Result = AttendanceRecord.SS;
                        attendance.Result_Am = AttendanceRecord.SS;
                        attendance.Result_Pm = AttendanceRecord.SS;
                        attendance.BeginIP = workDate.BeginIP;
                        attendance.EndIP = workDate.EndIP;
                        attendance.Date_Start = workDate.WorkBeginDate;
                        attendance.Date_End = workDate.WorkEndDate;
                    }

                    //如果还有其他未考虑到的异常情况，修正为正常考勤数据
                    if (attendance.Result == null)
                    {
                        var aBan = schedulings.Single(item => item.Name == "A班");
                        DateTime amStartTime = new DateTime(year, month, i, aBan.AmStartTime.Value.Hours, aBan.AmStartTime.Value.Minutes, aBan.AmStartTime.Value.Seconds);
                        DateTime pmEndTime = new DateTime(year, month, i, aBan.PmEndTime.Hours, aBan.PmEndTime.Minutes, aBan.PmEndTime.Seconds);

                        attendance.SchedulingID = aBan.ID;
                        attendance.Result = AttendanceRecord.Normal;
                        attendance.Result_Am = AttendanceRecord.Normal;
                        attendance.Result_Pm = AttendanceRecord.Normal;
                        attendance.BeginIP = workDate.BeginIP;
                        attendance.EndIP = workDate.EndIP;
                        attendance.Date_Start = workDate.WorkBeginDate < amStartTime ? workDate.WorkBeginDate : Utils.DateUtil.GetAmStartTime(year, month, i);
                        attendance.Date_End = workDate.WorkEndDate >= pmEndTime ? workDate.WorkEndDate : Utils.DateUtil.GetPmEndTime(year, month, i);
                        attendance.Date_Am_Start = attendance.Date_Start;
                        attendance.Date_Pm_End = attendance.Date_End;
                    }

                    this.attendances.Add(attendance);
                }

                Console.WriteLine("=============================================");
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public AttendanceService2 Encapsule()
        {
            foreach (var attend in attendances)
            {
                var myApply = attend.ToMyApplication();

                if (myApply != null)
                {
                    //申请
                    applications.Add(myApply.ToLinq());
                    //申请审批步骤
                    var steps = myApply.ToApplyVoteSteps();
                    applyVoteSteps.AddRange(steps);
                    myApply.UpdateDate = steps.Max(item => item.ModifyDate);
                    //申请审批日志
                    logsApplyVoteSteps.AddRange(myApply.ToLogs_ApplyVoteSteps(steps));
                    //受众
                    mapsAppStaff.Add(myApply.ToMapsAppStaff());

                    if (myApply.PrivateType > 0)
                    {
                        //日程安排
                        if (attend.Result.Contains("(上午)"))
                        {
                            var schedule = myApply.ToSchedule();
                            schedules.Add(schedule);
                            schedulesPrivate.Add(myApply.ToSchedulePrivate(schedule.ID, AmOrPm.Am));
                        }
                        else if (attend.Result.Contains("(下午)"))
                        {
                            var schedule = myApply.ToSchedule();
                            schedules.Add(schedule);
                            schedulesPrivate.Add(myApply.ToSchedulePrivate(schedule.ID, AmOrPm.Pm));
                        }
                        else
                        {
                            var scheduleAm = myApply.ToSchedule();
                            schedules.Add(scheduleAm);
                            schedulesPrivate.Add(myApply.ToSchedulePrivate(scheduleAm.ID, AmOrPm.Am));

                            var schedulePm = myApply.ToSchedule();
                            schedules.Add(schedulePm);
                            schedulesPrivate.Add(myApply.ToSchedulePrivate(schedulePm.ID, AmOrPm.Pm));
                        }
                    }
                }

                if (attend.Date_Start != null && attend.Date_End != null)
                {
                    //打卡记录
                    logsAttend.AddRange(attend.ToLogs_Attend());
                }
                //考勤结果
                pastsAttend.AddRange(attend.ToPasts_Attend());
            }

            var ids = Underly.PKeyType.AttendLog.Pick(logsAttend.Count);
            for (int i = 0; i < logsAttend.Count; i++)
                logsAttend[i].ID = ids[i];

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            using (var conn = ConnManager.Current.PvbErm)
            {
                //申请
                conn.BulkInsert(applications);
                conn.BulkInsert(applyVoteSteps);
                conn.BulkInsert(logsApplyVoteSteps);
                conn.BulkInsert(mapsAppStaff);

                //日程安排
                conn.BulkInsert(schedules);
                conn.BulkInsert(schedulesPrivate);

                //考勤
                conn.BulkInsert(logsAttend);
                conn.BulkInsert(pastsAttend);
            }
        }
    }
}
