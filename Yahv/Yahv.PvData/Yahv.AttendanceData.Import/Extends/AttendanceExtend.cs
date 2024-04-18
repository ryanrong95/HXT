using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;

namespace Yahv.AttendanceData.Import.Extends
{
    /// <summary>
    /// 考勤记录扩展方法
    /// </summary>
    public static class AttendanceExtend
    {
        /// <summary>
        /// 生成申请数据
        /// </summary>
        /// <param name="attendance">考勤数据</param>
        /// <returns></returns>
        public static Models.MyApplication ToMyApplication(this Models.Attendance attendance)
        {
            var staff = DataManager.Current.XdtStaffs.Single(item => item.Name == attendance.Name);
            var manager = DataManager.Current.XdtStaffs.Single(item => item.DepartmentType == staff.DepartmentType && item.PostType != PostType.Staff);

            Models.MyVoteFlow voteFlow = null;
            string title = null;
            string context = null;
            SchedulePrivateType spType = 0;
            DateLengthType dlType;
            decimal day = 0;
            switch (attendance.Result)
            {
                case AttendanceRecord.Overtime:
                    voteFlow = DataManager.Current[ApplicationType.Overtime];
                    title = $"{attendance.Name}的加班申请";
                    spType = SchedulePrivateType.Overtime;
                    context = new Models.OverTimeContext().ToJson(attendance.Date, manager.AdminID, manager.Name, manager.DyjCode, "申请加班");
                    break;

                case AttendanceRecord.CasualLeave:
                case AttendanceRecord.CasualLeave_AM:
                case AttendanceRecord.CasualLeave_PM:
                    day = attendance.Result == AttendanceRecord.CasualLeave ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.Offtime, day];
                    title = $"{attendance.Name}的事假申请";
                    spType = SchedulePrivateType.CasualLeave;
                    dlType = attendance.Result == AttendanceRecord.CasualLeave ? DateLengthType.AllDay : 
                             attendance.Result == AttendanceRecord.CasualLeave_AM ? DateLengthType.Morning : DateLengthType.Afternoon;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.SickLeave:
                case AttendanceRecord.SickLeave_AM:
                case AttendanceRecord.SickLeave_PM:
                    day = attendance.Result == AttendanceRecord.SickLeave ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.Offtime, day];
                    title = $"{attendance.Name}的病假申请";
                    spType = SchedulePrivateType.SickLeave;
                    dlType = attendance.Result == AttendanceRecord.SickLeave ? DateLengthType.AllDay :
                             attendance.Result == AttendanceRecord.SickLeave_AM ? DateLengthType.Morning : DateLengthType.Afternoon;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.PaidLeave:
                case AttendanceRecord.PaidLeave_AM:
                case AttendanceRecord.PaidLeave_PM:
                    day = attendance.Result == AttendanceRecord.PaidLeave ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.Offtime, 1m];
                    title = $"{attendance.Name}的带薪休假申请";
                    spType = SchedulePrivateType.AnnualLeave;
                    dlType = attendance.Result == AttendanceRecord.PaidLeave ? DateLengthType.AllDay :
                             attendance.Result == AttendanceRecord.PaidLeave_AM ? DateLengthType.Morning : DateLengthType.Afternoon;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.OB:
                case AttendanceRecord.OB_AM:
                case AttendanceRecord.OB_PM:
                    day = attendance.Result == AttendanceRecord.OB ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.Offtime, 1m];
                    title = $"{attendance.Name}的公务申请";
                    spType = SchedulePrivateType.OfficialBusiness;
                    dlType = attendance.Result == AttendanceRecord.OB ? DateLengthType.AllDay :
                             attendance.Result == AttendanceRecord.OB_AM ? DateLengthType.Morning : DateLengthType.Afternoon;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.BusinessTrip:
                case AttendanceRecord.BusinessTrip_AM:
                case AttendanceRecord.BusinessTrip_PM:
                    day = attendance.Result == AttendanceRecord.BusinessTrip ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.Offtime, 1m];
                    title = $"{attendance.Name}的公差申请";
                    spType = SchedulePrivateType.BusinessTrip;
                    dlType = attendance.Result == AttendanceRecord.BusinessTrip ? DateLengthType.AllDay :
                             attendance.Result == AttendanceRecord.BusinessTrip_AM ? DateLengthType.Morning : DateLengthType.Afternoon;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.MaternityLeave:
                    voteFlow = DataManager.Current[ApplicationType.Offtime, 1m];
                    title = $"{attendance.Name}的产假申请";
                    spType = SchedulePrivateType.MaternityLeave;
                    dlType = DateLengthType.AllDay;
                    context = new Models.OffTimeContext().ToJson(spType, manager.AdminID, manager.Name, manager.DyjCode, day, attendance.Date, dlType);
                    break;

                case AttendanceRecord.SS:
                case AttendanceRecord.SS_AM:
                case AttendanceRecord.SS_PM:
                    day = attendance.Result == AttendanceRecord.SS ? 1m : 0.5m;
                    voteFlow = DataManager.Current[ApplicationType.ReSign];
                    title = $"{attendance.Name}的补签申请";
                    spType = SchedulePrivateType.ReSign;
                    dlType = attendance.Result == AttendanceRecord.SS ? DateLengthType.AllDay :
                             attendance.Result == AttendanceRecord.SS_AM ? DateLengthType.Morning : DateLengthType.Afternoon;

                    bool amOn = false, amOff = false, pmOn = false, pmOff = false;
                    if (attendance.Result == AttendanceRecord.SS)
                    {
                        amOn = true;
                        pmOff = true;
                    }
                    else if (attendance.Result == AttendanceRecord.SS_AM)
                    {
                        amOn = true;
                        amOff = true;
                    }
                    else
                    {
                        pmOn = true;
                        pmOff = true;
                    }

                    context = new Models.ReSignContext().ToJson(attendance.Date, manager.AdminID, manager.Name, manager.DyjCode, amOn, amOff, pmOn, pmOff);
                    break;

                default:
                    break;
            }

            if (voteFlow != null)
            {
                var date = attendance.Date;
                //随机定义一个申请的创建时间
                var applyCreateDate = Utils.DateUtil.GetAmStartTime(date.Year, date.Month, date.Day).AddDays(-1).AddHours(1);
                var myApplication = new Models.MyApplication()
                {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.Erm_Application),
                    VoteFlow = voteFlow,
                    Title = title,
                    Context = context,
                    ApplicantID = staff.AdminID,
                    CreatorID = "NPC",
                    CreateDate = applyCreateDate,
                    Status = ApplicationStatus.Complete,

                    PrivateType = spType,
                    StaffID = attendance.StaffID,
                    ManagerID = manager.AdminID,
                    AttendDate = attendance.Date,
                };

                return myApplication;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 打卡记录
        /// </summary>
        /// <param name="attendance">考勤数据</param>
        /// <returns></returns>
        public static List<Layers.Data.Sqls.PvbErm.Logs_Attend> ToLogs_Attend(this Models.Attendance attendance)
        {
            var logsAttend = new List<Layers.Data.Sqls.PvbErm.Logs_Attend>();

            if (attendance.Date_Start.Value != attendance.Date_End.Value)
            {
                var logAttend_Start = new Layers.Data.Sqls.PvbErm.Logs_Attend()
                {
                    ID = null,
                    Date = attendance.Date,
                    StaffID = attendance.StaffID,
                    CreateDate = attendance.Date_Start.Value,
                    IP = attendance.BeginIP
                };
                logsAttend.Add(logAttend_Start);

                var logAttend_End = new Layers.Data.Sqls.PvbErm.Logs_Attend()
                {
                    ID = null,
                    Date = attendance.Date,
                    StaffID = attendance.StaffID,
                    CreateDate = attendance.Date_End.Value,
                    IP = attendance.EndIP
                };
                logsAttend.Add(logAttend_End);
            }
            else
            {
                var logAttend = new Layers.Data.Sqls.PvbErm.Logs_Attend()
                {
                    ID = null,
                    Date = attendance.Date,
                    StaffID = attendance.StaffID,
                    CreateDate = attendance.Date_Start.Value,
                    IP = attendance.BeginIP
                };

                logsAttend.Add(logAttend);
            }

            return logsAttend;
        }

        /// <summary>
        /// 考勤记录
        /// </summary>
        /// <param name="attendance">考勤数据</param>
        /// <returns></returns>
        public static List<Layers.Data.Sqls.PvbErm.Pasts_Attend> ToPasts_Attend(this Models.Attendance attendance)
        {
            var pastsAttend = new List<Layers.Data.Sqls.PvbErm.Pasts_Attend>();
            //随机定义一个统计考勤结果的时间
            var attendDate = Utils.DateUtil.GetPmEndTime(attendance.Date.Year, attendance.Date.Month, attendance.Date.Day).AddHours(5);

            if (attendance.Result_Am != null)
            {
                var pastAttend_AM = new Layers.Data.Sqls.PvbErm.Pasts_Attend()
                {
                    ID = $"{attendance.Date.ToString("yyyyMMdd")}{AmOrPm.Am}{attendance.StaffID}",
                    Date = attendance.Date,
                    AmOrPm = AmOrPm.Am,
                    StaffID = attendance.StaffID,
                    SchedulingID = attendance.SchedulingID,
                    StartTime = attendance.Date_Am_Start,
                    EndTime = attendance.Date_Am_End,
                    CreateDate = attendDate,
                    ModifyDate = attendDate,
                    InFact = (int)attendance.Result_Am.MapToInFact(),
                    IsLater = attendance.Result_Am.Contains(AttendanceRecord.BeLate),
                    IsEarly = attendance.Result_Am.Contains(AttendanceRecord.EarlyLeave),
                    OnWorkRemedy = attendance.Result_Am == AttendanceRecord.SS,
                    OffWorkRemedy = false
                };
                pastsAttend.Add(pastAttend_AM);
            }

            if (attendance.Result_Pm != null)
            {
                var pastAttend_PM = new Layers.Data.Sqls.PvbErm.Pasts_Attend()
                {
                    ID = $"{attendance.Date.ToString("yyyyMMdd")}{AmOrPm.Pm}{attendance.StaffID}",
                    Date = attendance.Date,
                    AmOrPm = AmOrPm.Pm,
                    StaffID = attendance.StaffID,
                    SchedulingID = attendance.SchedulingID,
                    StartTime = null,
                    EndTime = attendance.Date_Pm_End,
                    CreateDate = attendDate,
                    ModifyDate = attendDate,
                    InFact = (int)attendance.Result_Pm.MapToInFact(),
                    IsLater = attendance.Result_Pm.Contains(AttendanceRecord.BeLate),
                    IsEarly = attendance.Result_Pm.Contains(AttendanceRecord.EarlyLeave),
                    OnWorkRemedy = false,
                    OffWorkRemedy = attendance.Result_Pm == AttendanceRecord.SS
                };
                pastsAttend.Add(pastAttend_PM);
            }

            return pastsAttend;
        }

        /// <summary>
        /// 转换成实际考勤情况的枚举
        /// </summary>
        /// <param name="result">考勤结果</param>
        /// <returns>考勤情况枚举</returns>
        private static AttendInFactType MapToInFact(this string result)
        {
            var inFact = AttendInFactType.Normal;

            switch (result)
            {
                case AttendanceRecord.Normal:
                case AttendanceRecord.BeLate:
                case AttendanceRecord.EarlyLeave:
                case AttendanceRecord.BL_EL:
                case AttendanceRecord.SS:
                    inFact = AttendInFactType.Normal;
                    break;

                case AttendanceRecord.CasualLeave:
                    inFact = AttendInFactType.CasualLeave;
                    break;

                case AttendanceRecord.SickLeave:
                    inFact = AttendInFactType.SickLeave;
                    break;

                case AttendanceRecord.PaidLeave:
                case AttendanceRecord.MaternityLeave:
                    inFact = AttendInFactType.PaidLeave;
                    break;

                case AttendanceRecord.OB:
                    inFact = AttendInFactType.OfficialBusiness;
                    break;

                case AttendanceRecord.BusinessTrip:
                    inFact = AttendInFactType.BusinessTrip;
                    break;

                case AttendanceRecord.Overtime:
                    inFact = AttendInFactType.Overtime;
                    break;

                case AttendanceRecord.Absenteeism:
                    inFact = AttendInFactType.Absenteeism;
                    break;

                case AttendanceRecord.PublicHoliday:
                    inFact = AttendInFactType.PublicHoliday;
                    break;

                case AttendanceRecord.LegalHolidays:
                    inFact = AttendInFactType.LegalHolidays;
                    break;

                case AttendanceRecord.SA:
                    inFact = AttendInFactType.SystemAuthorizing;
                    break;

                default:
                    inFact = AttendInFactType.Normal;
                    break;
            }

            return inFact;
        }
    }
}
