using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;
using Yahv.Underly;

namespace Yahv.AttendanceData.Import.Services
{
    /// <summary>
    /// 考勤历史数据导入
    /// </summary>
    public partial class AttendanceService
    {
        #region 构造器

        RegexOptions options;
        Regex regex;

        public AttendanceService()
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

    public partial class AttendanceService : IDataService
    {
        /// <summary>
        /// 数据读取
        /// </summary>
        /// <returns></returns>
        public IDataService Read(string path = null)
        {
            //芯达通员工
            var xdtStaffs = DataManager.Current.XdtStaffs;
            //不需要按照公司规定打卡的员工
            var specialStaffs = System.Configuration.ConfigurationManager.AppSettings["SpecialStaffs"].Split(',');

            var dateArr = Path.GetFileNameWithoutExtension(path).Split('-');
            int year = int.Parse(dateArr[0]);
            int month = int.Parse(dateArr[1]);

            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                int lineIndex = 0;
                string headerLine = sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    lineIndex++;

                    var datas = line.Split(',');
                    var dyjCode = datas[0].Replace("\"", "").Trim(); //大赢家编码
                    var name = datas[1].Replace("\"", "").Trim(); //员工姓名

                    Console.WriteLine($"UserID: {dyjCode} Name: {name}");

                    var staff = xdtStaffs.SingleOrDefault(item => item.DyjCode == dyjCode && item.Name == name);
                    //如果当前员工在系统中没有配置，不能进行后续处理
                    if (staff == null)
                    {
                        Console.WriteLine($"UserID: {dyjCode} Name: {name} 在系统中未做配置");
                        Console.WriteLine("=============================================");
                        continue;
                    }

                    int days = DateTime.DaysInMonth(year, month);
                    for (int i = 1; i <= days; i++)
                    {
                        DateTime date = DateTime.Parse($"{year}-{month}-{i}").Date;
                        if (date < staff.HireDate)
                        {
                            continue;
                        }

                        var dayOfWeek = new DateTime(year, month, i).DayOfWeek;
                        var data = datas[i + 1].Replace("\"", "").Trim();
                        var attendance = new Models.Attendance()
                        {
                            StaffID = staff.ID,
                            Name = staff.Name,
                            DyjCode = staff.DyjCode,
                            RegionID = staff.RegionID,
                            SchedulingID = staff.SchedulingID,
                            Date = date,
                        };

                        //有考勤记录
                        if (!string.IsNullOrEmpty(data))
                        {
                            //有打卡记录
                            if (data.Contains("["))
                            {
                                var groups = regex.Match(data).Groups;
                                //实际考勤结果
                                string result = groups[1].Value;
                                //实际打卡时间
                                DateTime date_Start = new DateTime(year, month, i, int.Parse(groups[2].Value), int.Parse(groups[3].Value), 0);
                                DateTime date_End = new DateTime(year, month, i, int.Parse(groups[4].Value), int.Parse(groups[5].Value), 0);
                                //规定打卡时间
                                DateTime date_Am_Start = new DateTime(year, month, i, 9, 10, 0);
                                DateTime date_Am_End = new DateTime(year, month, i, 12, 0, 0);
                                DateTime date_Pm_Start = new DateTime(year, month, i, 13, 0, 0);
                                DateTime date_Pm_End = new DateTime(year, month, i, 17, 50, 0);

                                attendance.Result = result;
                                attendance.Date_Start = date_Start;
                                attendance.Date_End = date_End;

                                switch (result)
                                {
                                    case AttendanceRecord.Normal:
                                        attendance.Result_Am = AttendanceRecord.Normal;
                                        attendance.Result_Pm = AttendanceRecord.Normal;
                                        attendance.Date_Am_Start = date_Start;
                                        attendance.Date_Pm_End = date_End;
                                        break;

                                    case AttendanceRecord.BeLate:
                                    case AttendanceRecord.EarlyLeave:
                                    case AttendanceRecord.BL_EL:
                                        if (specialStaffs.Contains(attendance.Name))
                                        {
                                            attendance.Result_Am = AttendanceRecord.Normal;
                                            attendance.Result_Pm = AttendanceRecord.Normal;
                                            attendance.Date_Am_Start = new DateTime(year, month, i, 9, 0, 0);
                                            attendance.Date_Pm_End = new DateTime(year, month, i, 18, 0, 0);
                                        }
                                        else
                                        {
                                            attendance.Result_Am = GetResult_Am(date_Start, date_End, date_Am_Start, date_Am_End);
                                            attendance.Result_Pm = GetResult_Pm(date_Start, date_End, date_Pm_Start, date_Pm_End, date_Am_End);
                                            if (attendance.Result_Am == AttendanceRecord.SS)
                                            {
                                                attendance.Date_Pm_Start = date_Start;
                                                attendance.Date_Pm_End = date_End;
                                            }
                                            else if (attendance.Result_Pm == AttendanceRecord.SS)
                                            {

                                                attendance.Date_Am_Start = date_Start;
                                                attendance.Date_Am_End = date_End;
                                            }
                                            else
                                            {
                                                attendance.Date_Am_Start = date_Start;
                                                attendance.Date_Pm_End = date_End;
                                            }
                                        }
                                        break;

                                    case AttendanceRecord.CasualLeave_AM:
                                    case AttendanceRecord.SickLeave_AM:
                                    case AttendanceRecord.PaidLeave_AM:
                                    case AttendanceRecord.OB_AM:
                                        attendance.Result_Am = result.Replace("(上午)", "");
                                        attendance.Result_Pm = GetResult_Pm(date_Start, date_End, date_Pm_Start, date_Pm_End, date_Am_End);
                                        attendance.Date_Pm_Start = date_Start;
                                        attendance.Date_Pm_End = date_End;
                                        break;

                                    case AttendanceRecord.CasualLeave_PM:
                                    case AttendanceRecord.SickLeave_PM:
                                    case AttendanceRecord.PaidLeave_PM:
                                    case AttendanceRecord.OB_PM:
                                        attendance.Result_Am = GetResult_Am(date_Start, date_End, date_Am_Start, date_Am_End);
                                        attendance.Result_Pm = result.Replace("(下午)", ""); ;
                                        attendance.Date_Am_Start = date_Start;
                                        attendance.Date_Am_End = date_End;
                                        break;

                                    case AttendanceRecord.LegalHolidays:
                                        attendance.Result_Am = AttendanceRecord.Overtime;
                                        attendance.Result_Pm = AttendanceRecord.Overtime;
                                        attendance.Date_Am_Start = new DateTime(year, month, i, 9, 0, 0);
                                        attendance.Date_Pm_End = new DateTime(year, month, i, 18, 0, 0);
                                        break;

                                    case AttendanceRecord.SA:
                                        attendance.Result_Am = AttendanceRecord.SA;
                                        attendance.Result_Pm = AttendanceRecord.SA;
                                        attendance.Date_Am_Start = new DateTime(year, month, i, 9, 0, 0);
                                        attendance.Date_Pm_End = new DateTime(year, month, i, 18, 0, 0);
                                        break;

                                    default:
                                        break;
                                }

                                attendances.Add(attendance);
                            }
                            //无打卡记录，请假、公务、法定节假日、系统授权等
                            else
                            {
                                //系统授权，制造正常的上下班时间
                                if (data == AttendanceRecord.SA)
                                {
                                    attendance.Date_Am_Start = new DateTime(year, month, i, 9, 0, 0);
                                    attendance.Date_Pm_End = new DateTime(year, month, i, 18, 0, 0);
                                }
                                //考勤结果为"法定节假日 产假"、"法定节假日 陪产假"、"法定节假日 婚假"等情况
                                data = data.Replace(AttendanceRecord.LegalHolidays, "").Trim();

                                attendance.Result = data;
                                attendance.Result_Am = data;
                                attendance.Result_Pm = data;
                                attendances.Add(attendance);
                            }
                        }
                        //无考勤记录
                        else
                        {
                            //公休日
                            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                            {
                                attendance.Result_Am = AttendanceRecord.PublicHoliday;
                                attendance.Result_Pm = AttendanceRecord.PublicHoliday;
                                attendances.Add(attendance);
                            }
                            //工作日
                            else
                            {
                                attendance.Result_Am = AttendanceRecord.Absenteeism;
                                attendance.Result_Pm = AttendanceRecord.Absenteeism;
                                attendances.Add(attendance);
                            }
                        }
                    }

                    Console.WriteLine("=============================================");
                }
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public IDataService Encapsule()
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

        /// <summary>
        /// 获取上午考勤结果
        /// </summary>
        /// <param name="date_Start">员工实际打卡的开始时间</param>
        /// <param name="date_End">员工实际打卡的结束时间</param>
        /// <param name="date_Am_Start">公司规定的上午打卡开始时间</param>
        /// <param name="date_Am_End">公司规定的上午打卡结束时间</param>
        /// <returns>考勤结果</returns>
        private string GetResult_Am(DateTime date_Start, DateTime date_End, DateTime date_Am_Start, DateTime date_Am_End)
        {
            //如果打卡开始时间晚于规定的上午打卡结束时间，则上午无考勤记录，记为"补签"
            if (date_Start >= date_Am_End)
                return AttendanceRecord.SS;

            //上午有打卡记录，则进行分区间判断考勤结果
            if (date_Start < date_Am_Start && date_End >= date_Am_End)
                return AttendanceRecord.Normal;

            if (date_Start >= date_Am_Start && date_End >= date_Am_End)
                return AttendanceRecord.BeLate;

            if (date_Start < date_Am_Start && date_End < date_Am_End)
                return AttendanceRecord.EarlyLeave;

            if (date_Start >= date_Am_Start && date_End < date_Am_End)
                return AttendanceRecord.BL_EL;

            return AttendanceRecord.Normal;
        }

        /// <summary>
        /// 获取下午考勤结果
        /// </summary>
        /// <param name="date_Start">员工实际打卡的开始时间</param>
        /// <param name="date_End">员工实际打卡的结束时间</param>
        /// <param name="date_Pm_Start">公司规定的下午打卡开始时间</param>
        /// <param name="date_Pm_End">公司规定的下午打卡结束时间</param>
        /// <param name="date_Am_End">公司规定的上午打卡结束时间</param>
        /// <returns>考勤结果</returns>
        private string GetResult_Pm(DateTime date_Start, DateTime date_End, DateTime date_Pm_Start, DateTime date_Pm_End, DateTime date_Am_End)
        {
            //特殊情况校验，如果两次打卡时间在这个特殊区间[12:00, 13:00)，既不属于上午考勤时间，也属于下午考勤时间，则认为上午"补签"，下午"早退"
            if (date_Start >= date_Am_End && date_End < date_Pm_Start)
                return AttendanceRecord.EarlyLeave;

            //如果打卡结束时间早于规定的下午打卡开始时间，则下午无考勤记录，记为补签
            if (date_End < date_Pm_Start)
                return AttendanceRecord.SS;

            if (date_Start < date_Pm_Start && date_End >= date_Pm_End)
                return AttendanceRecord.Normal;

            if (date_Start >= date_Pm_Start && date_End >= date_Pm_End)
                return AttendanceRecord.BeLate;

            if (date_Start < date_Pm_Start && date_End < date_Pm_End)
                return AttendanceRecord.EarlyLeave;

            if (date_Start >= date_Pm_Start && date_End < date_Pm_End)
                return AttendanceRecord.BL_EL;

            return AttendanceRecord.Normal;
        }
    }
}
