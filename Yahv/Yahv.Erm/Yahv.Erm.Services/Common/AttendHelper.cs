using Layers.Data.Sqls;
using Layers.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services.Common
{
    public static class StaffHelper
    {
        /// <summary>
        /// 公司员工花名册
        /// </summary>
        /// <param name="filePath">保存路径</param>
        /// <param name="TempletePath">模板路径</param>
        public static void ExportRoster(string filePath, string TempletePath, DateTime date = default(DateTime))
        {
            var staffs = new Views.StaffAlls().Where(item => item.Labour.EnterpriseID == ErmConfig.LabourEnterpriseID || item.Labour.EnterpriseID == ErmConfig.LabourEnterpriseID2);
            //正常员工
            var onStaffs = staffs.Where(item =>
            (item.Status == StaffStatus.Normal && item.Labour.EntryDate <= date) ||
            (item.Status == StaffStatus.Period && item.Labour.EntryDate <= date) ||
            (item.Status == StaffStatus.Departure && item.Labour.LeaveDate >= date) ||
            (item.Status == StaffStatus.Cancel && item.Labour.LeaveDate >= date)).ToArray();
            //离职员工
            var offStaffs = staffs.Where(item =>
            (item.Status == StaffStatus.Departure && item.Labour.LeaveDate < date) ||
            (item.Status == StaffStatus.Cancel && item.Labour.LeaveDate < date)).ToArray();

            //创建数据字典
            var info1 = new Dictionary<object, int[]>();
            var info2 = new Dictionary<object, int[]>();

            for (int i = 3; i < onStaffs.Length + 3; i++)
            {
                var staff = onStaffs[i - 3];
                info1.Add((i - 2).ToString() + i + "=0", new int[] { i, 0 });//序号
                info1.Add(staff.SelCode + i + "=1", new int[] { i, 1 });
                info1.Add(staff.Name + i + "=2", new int[] { i, 2 });
                info1.Add(staff.Gender.GetDescription() + i + "=3", new int[] { i, 3 });
                info1.Add(staff.Personal.BirthDate?.ToShortDateString() + i + "=4", new int[] { i, 4 });
                info1.Add(staff.Personal.Volk + i + "=5", new int[] { i, 5 });

                info1.Add((staff.Personal.IsMarry == true ? "已婚" : "未婚").ToString() + i + "=6", new int[] { i, 6 });
                info1.Add(staff.Personal.PassAddress + i + "=7", new int[] { i, 7 });
                info1.Add(staff.Personal.HomeAddress + i + "=8", new int[] { i, 8 });
                info1.Add(staff.Personal.IDCard + i + "=9", new int[] { i, 9 });
                info1.Add(staff.Personal.Education + i + "=10", new int[] { i, 10 });

                info1.Add(staff.Personal.GraduatInstitutions + i + "=11", new int[] { i, 11 });
                info1.Add(staff.Personal.Major + i + "=12", new int[] { i, 12 });
                info1.Add(staff.Personal.Mobile + i + "=13", new int[] { i, 13 });
                info1.Add(staff.Personal.EmergencyContact + i + "=14", new int[] { i, 14 });
                info1.Add(staff.Personal.EmergencyMobile + i + "=15", new int[] { i, 15 });

                info1.Add(staff.Labour.EntryDate.ToShortDateString() + i + "=16", new int[] { i, 16 });
                if (string.IsNullOrEmpty(staff.DepartmentCode))
                {
                    info1.Add("" + i + "=17", new int[] { i, 17 });
                }
                else
                {
                    info1.Add(((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)).GetDescription() + i + "=17", new int[] { i, 17 });
                }
                if (staff.Postion == null)
                {
                    throw new Exception("员工" + staff.Name + "岗位未设置岗位");
                }
                info1.Add(staff.Postion?.Name + i + "=18", new int[] { i, 18 });
                info1.Add(staff.Labour.ContractPeriod?.ToShortDateString() + i + "=19", new int[] { i, 19 });
                info1.Add(staff.Personal.Summary + i + "=20", new int[] { i, 20 });
            }

            for (int i = 3; i < offStaffs.Length + 3; i++)
            {
                var staff = offStaffs[i - 3];
                info2.Add((i - 2).ToString() + i + "=0", new int[] { i, 0 });//序号
                info2.Add(staff.SelCode + i + "=1", new int[] { i, 1 });
                info2.Add(staff.Name + i + "=2", new int[] { i, 2 });
                info2.Add(staff.Gender.GetDescription() + i + "=3", new int[] { i, 3 });
                info2.Add(staff.Personal.BirthDate?.ToShortDateString() + i + "=4", new int[] { i, 4 });
                info2.Add(staff.Personal.Volk + i + "=5", new int[] { i, 5 });

                info2.Add((staff.Personal.IsMarry == true ? "已婚" : "未婚").ToString() + i + "=6", new int[] { i, 6 });
                info2.Add(staff.Personal.PassAddress + i + "=7", new int[] { i, 7 });
                info2.Add(staff.Personal.HomeAddress + i + "=8", new int[] { i, 8 });
                info2.Add(staff.Personal.IDCard + i + "=9", new int[] { i, 9 });
                info2.Add(staff.Personal.Education + i + "=10", new int[] { i, 10 });

                info2.Add(staff.Personal.GraduatInstitutions + i + "=11", new int[] { i, 11 });
                info2.Add(staff.Personal.Major + i + "=12", new int[] { i, 12 });
                info2.Add(staff.Personal.Mobile + i + "=13", new int[] { i, 13 });
                info2.Add(staff.Personal.EmergencyContact + i + "=14", new int[] { i, 14 });
                info2.Add(staff.Personal.EmergencyMobile + i + "=15", new int[] { i, 15 });

                info2.Add(staff.Labour.EntryDate.ToShortDateString() + i + "=16", new int[] { i, 16 });
                if (string.IsNullOrEmpty(staff.DepartmentCode))
                {
                    info2.Add("" + i + "=17", new int[] { i, 17 });
                }
                else
                {
                    info2.Add(((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)).GetDescription() + i + "=17", new int[] { i, 17 });
                }
                info2.Add(staff.Postion.Name + i + "=18", new int[] { i, 18 });
                info2.Add(staff.Labour.ContractPeriod?.ToShortDateString() + i + "=19", new int[] { i, 19 });
                info2.Add(staff.Labour.LeaveDate?.ToShortDateString() + i + "=20", new int[] { i, 20 });
                info2.Add(staff.Personal.Summary + i + "=21", new int[] { i, 21 });
            }

            //获取模板
            FileStream file = new FileStream(TempletePath, FileMode.Open, FileAccess.Read);
            //用模板创建对象
            IWorkbook xssfworkbook = new XSSFWorkbook(file);
            //获取模板中的Sheet
            ISheet sheet1 = xssfworkbook.GetSheet("在职");
            ISheet sheet2 = xssfworkbook.GetSheet("离职");

            //填充数据到表格
            foreach (var dic in info1)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //填充数据到表格
            foreach (var dic in info2)
            {
                if (sheet2.GetRow(dic.Value[0]) == null)
                    sheet2.CreateRow(dic.Value[0]);

                if (sheet2.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet2.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet2.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));
            }

            //保存新文件
            using (FileStream newFile = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                xssfworkbook.Write(newFile);
                newFile.Close();
            }
        }

        /// <summary>
        /// 批量设置班别
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="adminIds"></param>
        public static void UpdateScheduling(string scheduleId, string staffID)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                throw new Exception("班别为空，设置失败");
            }
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Staffs>(new
                {
                    SchedulingID = scheduleId
                }, item => item.ID == staffID);
            }
        }
    }

    public static class AttendHelper
    {
        /// <summary>
        /// 获取某月考勤的开始日期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime GetAttendStartDate(int year, int month)
        {
            if (month == 1)
            {
                return new DateTime(year - 1, 12, 26);
            }
            else
            {
                return new DateTime(year, month - 1, 26);
            }
        }

        /// <summary>
        /// 获取某月考勤的结束日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetAttendEndDate(int year, int month)
        {
            return new DateTime(year, month, 25);
        }

        /// <summary>
        ///  获取某月考勤的开始日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetAttendStateDate(DateTime date)
        {
            var Year = date.Year;
            var Month = date.Month;
            var Day = date.Day;

            if (Day <= 25)
            {
                if (Month == 1)
                {
                    return new DateTime(Year - 1, 12, 26);
                }
                else
                {
                    return new DateTime(Year, Month - 1, 26);
                }
            }
            else
            {
                return new DateTime(Year, Month, 26);
            }
        }

        /// <summary>
        /// 获取某月考勤的结束日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetAttendEndDate(DateTime date)
        {
            var Year = date.Year;
            var Month = date.Month;
            var Day = date.Day;

            if (Day <= 25)
            {
                return new DateTime(Year, Month, 25);
            }
            else
            {
                if (Month == 12)
                {
                    return new DateTime(Year + 1, 1, 25);
                }
                else
                {
                    return new DateTime(Year, Month + 1, 25);
                }
            }
        }

        /// <summary>
        /// 自动获取补签时间(上午上班、上午下班、下午上班、下午下班)
        /// </summary>
        /// <param name="AdminID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ReSignContext GetReSignContext(string AdminID, DateTime date)
        {
            #region 判断员工班别
            var scheduleid = string.Empty;
            var Staff = new StaffAlls().SingleOrDefault(item => item.Admin.ID == AdminID);
            var past = new PastsAttendOrigin().Where(item => item.StaffID == Staff.ID && item.Date == date).FirstOrDefault();
            if (past != null)
            {
                if (past.InFact == AttendInFactType.LegalHolidays || past.InFact == AttendInFactType.PublicHoliday)
                {
                    throw new Exception("非工作日不能申请补签");
                }
                //首先已考勤结果的班别为准
                scheduleid = past.SchedulingID;
            }
            else
            {
                var schedulePublic = new SchedulesPublicRoll().Single(item => item.ShiftID == Staff.SchedulingID && item.RegionID == Staff.RegionID && item.Date == date);
                if (schedulePublic.Method != Underly.Enums.ScheduleMethod.Work)
                {
                    throw new Exception("非工作日不能申请补签");
                }
                //其次已员工当时的班别为准
                scheduleid = schedulePublic.SchedulingID;
            }
            var scheduling = new SchedulingsOrigin().Single(item => item.ID == scheduleid);

            #endregion

            #region 私人请假日程            
            var schedulePrivate = new SchedulesPrivateOrigin()
                .Where(item => item.StaffID == Staff.ID && item.Date == date)
                .Where(item => item.SchedulePrivateType != Underly.Enums.SchedulePrivateType.Overtime && item.SchedulePrivateType != Underly.Enums.SchedulePrivateType.ReSign);
            var schedulePrivateAm = schedulePrivate.Count() == 0 ? null : schedulePrivate.SingleOrDefault(item => item.AmOrPm == Underly.Enums.AmOrPm.Am.ToString());
            var schedulePrivatePm = schedulePrivate.Count() == 0 ? null : schedulePrivate.SingleOrDefault(item => item.AmOrPm == Underly.Enums.AmOrPm.Pm.ToString());
            #endregion

            #region 员工打卡记录
            var Logs_Attend = new Logs_AttendOrigin().Where(item => item.StaffID == Staff.ID && item.Date == date).Select(item => item.CreateDate.TimeOfDay);
            var ReSignContext = new ReSignContext()
            {
                Date = date,
                Attends = Logs_Attend.ToList(),
            };
            #endregion

            #region 计算补签情况
            if (scheduling.AmStartTime == null && scheduling.AmEndTime == null && scheduling.PmStartTime != null && scheduling.PmEndTime != null)
            {
                //下午半天班的情况（C班）
                if (schedulePrivatePm == null)
                {
                    if (Logs_Attend.Count() == 0)
                    {
                        //整天没打卡，下午都需补签
                        ReSignContext.PmOn = true;
                        ReSignContext.PmOff = true;
                    }
                    else
                    {
                        var MinAttend = Logs_Attend.Min();//最早打卡时间
                        var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                        TimeSpan PmStartTime = new TimeSpan(scheduling.PmStartTime.Hours, scheduling.PmStartTime.Minutes + 20, scheduling.PmStartTime.Seconds);
                        TimeSpan PmEndTime = new TimeSpan(scheduling.PmEndTime.Hours, scheduling.PmEndTime.Minutes - 20, scheduling.PmEndTime.Seconds);
                        TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);
                        TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);

                        int value1 = attendMin.CompareTo(PmStartTime);
                        int value2 = PmEndTime.CompareTo(attendMax);
                        if (value1 > 0)
                        {
                            ReSignContext.PmOn = true;
                        }
                        if (value2 > 0)
                        {
                            ReSignContext.PmOff = true;
                        }
                    }

                }
                else
                {
                    throw new Exception("下午半天班并且已经请假，不需补签");
                }
            }
            else if (scheduling.AmStartTime != null && scheduling.AmEndTime != null && scheduling.PmStartTime == null && scheduling.PmEndTime == null)
            {
                //上午半天班的情况
            }
            else
            {
                //整天班的情况
                if (schedulePrivateAm == null && schedulePrivatePm == null)
                {
                    //没有请假
                    if (Logs_Attend.Count() == 0)
                    {
                        //整天没打卡，上下午都需补签
                        ReSignContext.AmOn = true;
                        ReSignContext.PmOff = true;
                    }
                    else
                    {
                        var MinAttend = Logs_Attend.Min();//最早打卡时间
                        var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                        var amStart = scheduling.AmStartTime.Value;
                        TimeSpan AmStartTime = new TimeSpan(amStart.Hours, amStart.Minutes + 20, amStart.Seconds);
                        TimeSpan PmEndTime = new TimeSpan(scheduling.PmEndTime.Hours, scheduling.PmEndTime.Minutes - 20, scheduling.PmEndTime.Seconds);
                        TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);
                        TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);

                        int value1 = attendMin.CompareTo(AmStartTime);
                        int value2 = PmEndTime.CompareTo(attendMax);
                        if (value1 > 0)
                        {
                            ReSignContext.AmOn = true;
                        }
                        if (value2 > 0)
                        {
                            ReSignContext.PmOff = true;
                        }
                    }
                }
                else if (schedulePrivateAm != null && schedulePrivatePm == null)
                {
                    //上午请假
                    if (Logs_Attend.Count() == 0)
                    {
                        //整天没打卡，下午都需补签
                        ReSignContext.PmOn = true;
                        ReSignContext.PmOff = true;
                    }
                    else
                    {
                        var MinAttend = Logs_Attend.Min();//最早打卡时间
                        var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                        TimeSpan PmStartTime = new TimeSpan(scheduling.PmStartTime.Hours, scheduling.PmStartTime.Minutes + 20, scheduling.PmStartTime.Seconds);
                        TimeSpan PmEndTime = new TimeSpan(scheduling.PmEndTime.Hours, scheduling.PmEndTime.Minutes - 20, scheduling.PmEndTime.Seconds);
                        TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);
                        TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);

                        int value1 = attendMin.CompareTo(PmStartTime);
                        int value2 = PmEndTime.CompareTo(attendMax);
                        if (value1 > 0)
                        {
                            ReSignContext.PmOn = true;
                        }
                        if (value2 > 0)
                        {
                            ReSignContext.PmOff = true;
                        }
                    }
                }
                else if (schedulePrivateAm == null && schedulePrivatePm != null)
                {
                    //下午请假
                    if (Logs_Attend.Count() == 0)
                    {
                        //整天没打卡，上午都需补签
                        ReSignContext.AmOn = true;
                        ReSignContext.AmOff = true;
                    }
                    else
                    {
                        var MinAttend = Logs_Attend.Min();//最早打卡时间
                        var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                        var amStart = scheduling.AmStartTime.Value;
                        var amEnd = scheduling.AmEndTime.Value;
                        TimeSpan AmStartTime = new TimeSpan(amStart.Hours, amStart.Minutes + 20, amStart.Seconds);
                        TimeSpan AmEndTime = new TimeSpan(amEnd.Hours, amEnd.Minutes - 20, amEnd.Seconds);
                        TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);
                        TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);

                        int value1 = attendMin.CompareTo(AmStartTime);
                        int value2 = AmEndTime.CompareTo(attendMax);
                        if (value1 > 0)
                        {
                            ReSignContext.AmOn = true;
                        }
                        if (value2 > 0)
                        {
                            ReSignContext.AmOff = true;
                        }
                    }
                }
                else
                {
                    //整天请假
                    throw new Exception("整天已经请假，不需补签");
                }
            }
            #endregion

            //本月已补签的次数
            var startDate = GetAttendStateDate(date);
            var endDate = GetAttendEndDate(date);
            var resign = new SchedulesPrivateOrigin()
                .Where(item => item.StaffID == Staff.ID && item.SchedulePrivateType == Underly.Enums.SchedulePrivateType.ReSign)
                .Where(item => item.Date >= startDate && item.Date <= endDate);
            var resigns1 = resign.Where(item => item.OnWorkRemedy == true).Count();
            var resigns2 = resign.Where(item => item.OffWorkRemedy == true).Count();
            ReSignContext.ReSignTimes = resigns1 + resigns2;

            return ReSignContext;
        }

        /// <summary>
        /// 获取员工某日的打卡时间
        /// </summary>
        public static string GeAttendTime(string AdminID, DateTime date)
        {
            //获取员工信息
            var Staff = new StaffAlls().SingleOrDefault(item => item.Admin.ID == AdminID);
            var Logs_Attend = new Logs_AttendOrigin().Where(item => item.StaffID == Staff.ID && item.Date == date).Select(item => item.CreateDate.TimeOfDay);
            if (Logs_Attend.Count() == 0)
            {
                return "--";
            }
            else
            {
                var min = Logs_Attend.Min();
                var max = Logs_Attend.Max();
                return min.ToString(@"hh\:mm\:ss") + " - " + max.ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// 计算个人的考勤结果
        /// </summary>
        /// <param name="StaffID"></param>
        /// <param name="date"></param>
        public static void CalculatePastAttends(string StaffID, DateTime date, string SchedulingID = "")
        {
            //获取员工信息
            var Staff = new StaffAlls().Single(item => item.ID == StaffID);
            //获取当日公共日程,如果SchedulingID为空，则获取员工当前的班别
            var schedulePublic = string.IsNullOrEmpty(SchedulingID) ?
                new SchedulesPublicRoll().Single(item => item.SchedulingID == Staff.SchedulingID && item.RegionID == Staff.RegionID && item.Date == date) : new SchedulesPublicRoll().Single(item => item.SchedulingID == SchedulingID && item.RegionID == Staff.RegionID && item.Date == date);
            //获取当日私有日程
            var schedulePrivate = new SchedulesPrivateOrigin().Where(item => item.StaffID == Staff.ID && item.Date == date);
            //获取员工班别
            var scheduling = new SchedulingsOrigin().Single(item => item.ID == schedulePublic.SchedulingID);
            //获取员工的打卡记录
            var Logs_Attend = new Logs_AttendOrigin().Where(item => item.StaffID == Staff.ID && item.Date == date).Select(item => item.CreateDate.TimeOfDay);

            //新建考勤结果
            var pasts = new PastsAttendOrigin().Where(item => item.StaffID == StaffID && item.Date == date);
            var am = pasts.FirstOrDefault(item => item.AmOrPm == AmOrPm.Am.ToString()) ??
                new PastsAttend() { Date = date, StaffID = StaffID, AmOrPm = AmOrPm.Am.ToString(), SchedulingID = schedulePublic.SchedulingID };
            var pm = pasts.FirstOrDefault(item => item.AmOrPm == AmOrPm.Pm.ToString()) ??
                new PastsAttend() { Date = date, StaffID = StaffID, AmOrPm = AmOrPm.Pm.ToString(), SchedulingID = schedulePublic.SchedulingID };

            if (schedulePublic.Method == ScheduleMethod.PublicHoliday)
            {
                //公休日,并判断是否有加班
                var count = schedulePrivate.Where(item => item.SchedulePrivateType == SchedulePrivateType.Overtime).Count();
                if (count == 0)
                {
                    am.InFact = AttendInFactType.PublicHoliday;
                    pm.InFact = AttendInFactType.PublicHoliday;
                }
                else
                {
                    am.InFact = AttendInFactType.Overtime;
                    pm.InFact = AttendInFactType.Overtime;
                }
            }
            else if (schedulePublic.Method == ScheduleMethod.LegalHoliday)
            {
                //法定假日,并判断是否有加班
                var count = schedulePrivate.Where(item => item.SchedulePrivateType == SchedulePrivateType.Overtime).Count();
                if (count == 0)
                {
                    am.InFact = AttendInFactType.LegalHolidays;
                    pm.InFact = AttendInFactType.LegalHolidays;
                }
                else
                {
                    am.InFact = AttendInFactType.Overtime;
                    pm.InFact = AttendInFactType.Overtime;
                }
            }
            else
            {
                var schedulePrivateAm = schedulePrivate.SingleOrDefault(item => item.AmOrPm == AmOrPm.Am.ToString());
                var schedulePrivatePm = schedulePrivate.SingleOrDefault(item => item.AmOrPm == AmOrPm.Pm.ToString());

                #region 工作日:下午上班
                if (scheduling.AmStartTime == null && scheduling.AmEndTime == null && scheduling.PmStartTime != null && scheduling.PmEndTime != null)
                {
                    //上午结果
                    am.InFact = AttendInFactType.Normal;
                    //下午结果
                    if (schedulePrivatePm == null)
                    {
                        //根据考勤时间计算结果
                        var count = Logs_Attend.Count();
                        if (count < 2)
                        {
                            if (schedulePrivatePm == null)
                            {
                                //打卡小于两次为旷工
                                pm.InFact = AttendInFactType.Absenteeism;
                            }
                            else
                            {
                                pm.InFact = ConvertAttendInFactType(schedulePrivatePm.SchedulePrivateType);
                            }
                        }
                        else
                        {
                            var MinAttend = Logs_Attend.Min();//最早打卡时间
                            var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                            var pmStart = scheduling.PmStartTime;
                            var pmEnd = scheduling.PmEndTime;

                            TimeSpan PmLaterTime = new TimeSpan(pmStart.Hours, pmStart.Minutes, pmStart.Seconds);//迟到时间
                            TimeSpan PmEarlyTime = new TimeSpan(pmEnd.Hours, pmEnd.Minutes, pmEnd.Seconds);//早退时间

                            TimeSpan PmStartTime = new TimeSpan(pmStart.Hours, pmStart.Minutes + 20, pmStart.Seconds);//上班旷工
                            TimeSpan PmEndTime = new TimeSpan(pmEnd.Hours, pmEnd.Minutes - 20, pmEnd.Seconds);//下班旷工

                            TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);//最早打卡时间
                            TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);//最晚打卡时间

                            int value1 = attendMin.CompareTo(PmStartTime);
                            int value2 = PmEndTime.CompareTo(attendMax);
                            int value3 = attendMin.CompareTo(PmLaterTime);
                            int value4 = PmEarlyTime.CompareTo(attendMax);
                            if (value1 > 0 || value2 > 0)
                            {
                                pm.InFact = AttendInFactType.Absenteeism;
                            }
                            else
                            {
                                pm.InFact = AttendInFactType.Normal;
                                if (value3 > 0)
                                {
                                    pm.IsLater = true;
                                }
                                if (value4 > 0)
                                {
                                    pm.IsEarly = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (schedulePrivatePm.SchedulePrivateType == SchedulePrivateType.ReSign)
                        {
                            pm.InFact = AttendInFactType.Normal;
                            pm.OnWorkRemedy = schedulePrivatePm.OnWorkRemedy;
                            pm.OffWorkRemedy = schedulePrivatePm.OffWorkRemedy;
                        }
                        else
                        {
                            pm.InFact = ConvertAttendInFactType(schedulePrivatePm.SchedulePrivateType);
                        }
                    }
                }
                #endregion

                #region 工作日:上午上班
                if (scheduling.AmStartTime != null && scheduling.AmEndTime != null && scheduling.PmStartTime == null && scheduling.PmEndTime == null)
                {
                    //上午结果
                    if (schedulePrivateAm == null)
                    {
                        //根据考勤时间计算结果
                        var count = Logs_Attend.Count();
                        if (count < 2)
                        {
                            if (schedulePrivateAm == null)
                            {
                                //打卡小于两次为旷工
                                am.InFact = AttendInFactType.Absenteeism;
                            }
                            else
                            {
                                am.InFact = ConvertAttendInFactType(schedulePrivateAm.SchedulePrivateType);
                            }
                        }
                        else
                        {
                            var MinAttend = Logs_Attend.Min();//最早打卡时间
                            var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                            var amStart = scheduling.AmStartTime.Value;
                            var amEnd = scheduling.AmEndTime.Value;

                            TimeSpan AmLaterTime = new TimeSpan(amStart.Hours, amStart.Minutes, amStart.Seconds);//迟到时间
                            TimeSpan AmEarlyTime = new TimeSpan(amEnd.Hours, amEnd.Minutes, amEnd.Seconds);//早退时间

                            TimeSpan AmStartTime = new TimeSpan(amStart.Hours, amStart.Minutes + 20, amStart.Seconds);//上班旷工
                            TimeSpan AmEndTime = new TimeSpan(amEnd.Hours, amEnd.Minutes - 20, amEnd.Seconds);//下班旷工

                            TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);//最早打卡时间
                            TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);//最晚打卡时间

                            int value1 = attendMin.CompareTo(AmStartTime);
                            int value2 = AmEndTime.CompareTo(attendMax);
                            int value3 = attendMin.CompareTo(AmLaterTime);
                            int value4 = AmEarlyTime.CompareTo(attendMax);
                            if (value1 > 0 || value2 > 0)
                            {
                                am.InFact = AttendInFactType.Absenteeism;
                            }
                            else
                            {
                                am.InFact = AttendInFactType.Normal;
                                if (value3 > 0)
                                {
                                    am.IsLater = true;
                                }
                                if (value4 > 0)
                                {
                                    am.IsEarly = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (schedulePrivateAm.SchedulePrivateType == SchedulePrivateType.ReSign)
                        {
                            am.InFact = AttendInFactType.Normal;
                            am.OnWorkRemedy = schedulePrivateAm.OnWorkRemedy;
                            am.OffWorkRemedy = schedulePrivateAm.OffWorkRemedy;
                        }
                        else
                        {
                            am.InFact = ConvertAttendInFactType(schedulePrivateAm.SchedulePrivateType);
                        }
                    }
                    //下午结果
                    pm.InFact = AttendInFactType.Normal;
                }
                #endregion

                #region 工作日：全天上班
                if (scheduling.AmStartTime != null && scheduling.AmEndTime != null && scheduling.PmStartTime != null && scheduling.PmEndTime != null)
                {
                    var count = Logs_Attend.Count();
                    if (count == 0)
                    {
                        if (schedulePrivateAm == null)
                        {
                            //未打卡
                            am.InFact = AttendInFactType.Absenteeism;
                        }
                        else
                        {
                            am.InFact = ConvertAttendInFactType(schedulePrivateAm.SchedulePrivateType);
                        }
                        if (schedulePrivatePm == null)
                        {
                            pm.InFact = AttendInFactType.Absenteeism;
                        }
                        else
                        {
                            pm.InFact = ConvertAttendInFactType(schedulePrivatePm.SchedulePrivateType);
                        }
                    }
                    else
                    {
                        var MinAttend = Logs_Attend.Min();//最早打卡时间
                        var MaxAttend = Logs_Attend.Max();//最晚打卡时间
                        TimeSpan attendMin = new TimeSpan(MinAttend.Hours, MinAttend.Minutes, MinAttend.Seconds);//最早打卡时间
                        TimeSpan attendMax = new TimeSpan(MaxAttend.Hours, MaxAttend.Minutes, MaxAttend.Seconds);//最晚打卡时间

                        //上午结果
                        if (schedulePrivateAm == null)
                        {
                            var amStart = scheduling.AmStartTime.Value;
                            TimeSpan AmLaterTime = new TimeSpan(amStart.Hours, amStart.Minutes, amStart.Seconds);//迟到时间
                            TimeSpan AmStartTime = new TimeSpan(amStart.Hours, amStart.Minutes + 20, amStart.Seconds);//上班旷工时间

                            int value1 = attendMin.CompareTo(AmLaterTime);
                            int value2 = attendMin.CompareTo(AmStartTime);
                            if (value2 > 0)
                            {
                                //上午旷工
                                am.InFact = AttendInFactType.Absenteeism;
                            }
                            else if (value1 > 0)
                            {
                                //上午迟到
                                am.InFact = AttendInFactType.Normal;
                                am.IsLater = true;
                            }
                            else
                            {
                                //上午正常
                                am.InFact = AttendInFactType.Normal;
                                am.IsLater = false;
                            }
                            //判断是否需要验证，上午下班的考勤
                            if (am.InFact != AttendInFactType.Absenteeism && schedulePrivatePm != null && schedulePrivatePm.SchedulePrivateType != SchedulePrivateType.ReSign)
                            {
                                var amEnd = scheduling.AmEndTime.Value;
                                var DomainValue = scheduling.DomainValue;
                                TimeSpan AmEarlyTime = new TimeSpan(amEnd.Hours, amEnd.Minutes, amEnd.Seconds);//上午早退时间
                                TimeSpan AmEndTime = new TimeSpan(amEnd.Hours, amEnd.Minutes - DomainValue, amEnd.Seconds);//上班旷工时间

                                int value3 = attendMax.CompareTo(AmEarlyTime);
                                int value4 = attendMax.CompareTo(AmEndTime);

                                if (value3 >= 0)
                                {
                                    //上午正常
                                    am.InFact = AttendInFactType.Normal;
                                    am.IsEarly = false;
                                }
                                else if (value4 >= 0)
                                {
                                    //上午早退
                                    am.InFact = AttendInFactType.Normal;
                                    am.IsEarly = true;
                                }
                                else
                                {
                                    //上午旷工
                                    am.InFact = AttendInFactType.Absenteeism;
                                    am.IsEarly = false;
                                    am.IsLater = false;
                                }
                            }
                        }
                        else
                        {
                            if (schedulePrivateAm.SchedulePrivateType == SchedulePrivateType.ReSign)
                            {
                                //上午补签
                                am.InFact = AttendInFactType.Normal;
                                am.OnWorkRemedy = schedulePrivateAm.OnWorkRemedy;
                                am.OffWorkRemedy = schedulePrivateAm.OffWorkRemedy;
                            }
                            else
                            {
                                //上午请假
                                am.InFact = ConvertAttendInFactType(schedulePrivateAm.SchedulePrivateType);
                            }
                        }
                        //下午结果
                        if (schedulePrivatePm == null)
                        {
                            var apmEnd = scheduling.PmEndTime;
                            TimeSpan pmLaterTime = new TimeSpan(apmEnd.Hours, apmEnd.Minutes, apmEnd.Seconds);//下班早退时间
                            TimeSpan pmEndTime = new TimeSpan(apmEnd.Hours, apmEnd.Minutes - scheduling.DomainValue, apmEnd.Seconds);//下班旷工时间

                            int value1 = pmLaterTime.CompareTo(attendMax);
                            int value2 = pmEndTime.CompareTo(attendMax);
                            if (value2 > 0)
                            {
                                //下午旷工
                                pm.InFact = AttendInFactType.Absenteeism;
                            }
                            else if (value1 > 0)
                            {
                                //下午早退
                                pm.InFact = AttendInFactType.Normal;
                                pm.IsEarly = true;
                            }
                            else
                            {
                                //下午正常
                                pm.InFact = AttendInFactType.Normal;
                                pm.IsEarly = false;
                            }

                            //判断是否需要验证，下午上班的考勤
                            if (pm.InFact != AttendInFactType.Absenteeism && schedulePrivateAm != null && schedulePrivateAm.SchedulePrivateType != SchedulePrivateType.ReSign)
                            {
                                var pmStart = scheduling.PmStartTime;
                                var DomainValue = scheduling.DomainValue;
                                TimeSpan PmLaterTime = new TimeSpan(pmStart.Hours, pmStart.Minutes, pmStart.Seconds);//下午迟到时间
                                TimeSpan PmEndTime = new TimeSpan(pmStart.Hours, pmStart.Minutes + DomainValue, pmStart.Seconds);//下午上班旷工时间

                                int value3 = attendMin.CompareTo(PmLaterTime);
                                int value4 = attendMin.CompareTo(PmEndTime);

                                if (value4 > 0)
                                {
                                    //下午旷工
                                    pm.InFact = AttendInFactType.Absenteeism;
                                    pm.IsEarly = false;
                                    pm.IsLater = false;
                                }
                                else if (value3 > 0)
                                {
                                    //下午迟到
                                    pm.InFact = AttendInFactType.Normal;
                                    pm.IsLater = true;
                                }
                                else
                                {
                                    //下午正常
                                    pm.InFact = AttendInFactType.Normal;
                                    pm.IsLater = false;
                                    pm.IsEarly = false;
                                }
                            }
                        }
                        else
                        {
                            if (schedulePrivatePm.SchedulePrivateType == SchedulePrivateType.ReSign)
                            {
                                pm.InFact = AttendInFactType.Normal;
                                pm.OnWorkRemedy = schedulePrivatePm.OnWorkRemedy;
                                pm.OffWorkRemedy = schedulePrivatePm.OffWorkRemedy;
                            }
                            else
                            {
                                pm.InFact = ConvertAttendInFactType(schedulePrivatePm.SchedulePrivateType);
                            }
                        }
                    }
                }
                #endregion
            }
            am.Enter();
            pm.Enter();
        }

        /// <summary>
        /// 个人日程类型转考勤结果类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AttendInFactType ConvertAttendInFactType(SchedulePrivateType type)
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
                //育儿假
                case SchedulePrivateType.ParentalLeave:
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
                //补签
                case SchedulePrivateType.ReSign:
                    result = AttendInFactType.Normal;
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 批量更新员工考勤班别
        /// </summary>
        /// <param name="staffid">员工ID</param>
        /// <param name="dates">日期</param>
        /// <param name="scheduleId">班别ID</param>
        public static void UpdatePastsAttendScheduling(string staffid, DateTime[] dates, string scheduleId)
        {
            if (string.IsNullOrEmpty(staffid))
            {
                throw new Exception("员工ID为空，设置失败");
            }
            if (dates == null || dates.Length == 0)
            {
                throw new Exception("日期参数为空，设置失败");
            }
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //更新考勤班别
                repository.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(new
                {
                    SchedulingID = scheduleId
                }, item => item.StaffID == staffid && dates.Contains(item.Date));

                foreach (var date in dates)
                {
                    //重新计算考勤结果
                    CalculatePastAttends(staffid, date, scheduleId);
                }
            }
        }

        public static void UpdatePastsAttendSysNormal(string staffid, DateTime[] dates)
        {
            if (string.IsNullOrEmpty(staffid))
            {
                throw new Exception("员工ID为空，设置失败");
            }
            if (dates == null || dates.Length == 0)
            {
                throw new Exception("日期参数为空，设置失败");
            }
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Pasts_Attend>(new
                {
                    InFact = AttendInFactType.SystemAuthorizing,
                    IsLater = false,
                    IsEarly = false,
                    OnWorkRemedy = false,
                    OffWorkRemedy = false,
                }, item => item.StaffID == staffid && dates.Contains(item.Date));
            }
        }
    }
}
