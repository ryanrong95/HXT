using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 芯达通员工假期情况视图
    /// </summary>
    public class StaffVacations_Xdt : UniqueView<StaffVacation_Show, PvbErmReponsitory>
    {
        private int year;
        private DateTime start;
        private DateTime end;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StaffVacations_Xdt(int year)
        {
            this.year = year;
            start = new DateTime(year, 1, 1);
            end = new DateTime(year + 1, 1, 1);
        }

        protected override IQueryable<StaffVacation_Show> GetIQueryable()
        {
            var staffView = new StaffsOrigin(this.Reponsitory).Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            var labourView = new LaboursOrigin(this.Reponsitory).Where(item => item.EnterpriseID == Common.ErmConfig.LabourEnterpriseID);
            var staffs = (from entity in staffView
                          join labour in labourView on entity.ID equals labour.ID
                          select entity).ToArray();

            var admins = new AdminsOrigin(this.Reponsitory);
            var vocationView = new VacationsOrigin(this.Reponsitory);
            var positionView = new PostionsOrigin(this.Reponsitory);

            var query = new ApplicationsOrigin(this.Reponsitory)
                .Where(t => t.Title == "请假申请")
                .Where(t => t.ApplicationStatus == ApplicationStatus.UnderApproval || t.ApplicationStatus == ApplicationStatus.Complete)
                .Select(t => new ApplicationContent()
                {
                    ApplicaitonID = t.ApplicantID,
                    Content = t.Context.JsonTo<OffTimeContent>(),
                }).ToArray();

            var querydates = query.SelectMany(t => t.Content.DateItems.Select(k => new OffTimeDate
            {
                ApplicaitonID = t.ApplicaitonID,
                LeaveType = t.Content.LeaveType,
                Date = k,
            })).Where(t => t.Date.Date >= this.start && t.Date.Date < this.end);

            var linq = from staff in staffs
                       join vocation in vocationView on staff.ID equals vocation.StaffID into vocations
                       join admin in admins on staff.ID equals admin.StaffID
                       join position in positionView on staff.PostionID equals position.ID
                       join offtimedate in querydates on admin.ID equals offtimedate.ApplicaitonID into offtimedates
                       select new StaffVacation_Show()
                       {
                           ID = staff.ID,
                           Name = staff.Name,
                           Code = staff.Code,
                           SelCode = staff.SelCode,
                           DepartmentCode = staff.DepartmentCode,
                           StaffVacations = vocations,
                           OffTimeDates = offtimedates,
                           Postion= position,
                       };
            return linq.AsQueryable();
        }
    }

    public class StaffVacation_Show : IUnique
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string SelCode { get; set; }

        public string DepartmentCode { get; set; }

        public Postion Postion { get; set; }

        //员工假期
        public IEnumerable<Vacation> StaffVacations { get; set; }

        //员工请假日期
        public IEnumerable<OffTimeDate> OffTimeDates { get; set; }

        /// <summary>
        /// 总年假
        /// </summary>
        public decimal? TotalYearDay
        {
            get
            {
                return this.StaffVacations.FirstOrDefault(t => t.Type == VacationType.YearsDay)?.Total;
            }
        }

        /// <summary>
        /// 总病假
        /// </summary>
        public decimal? TotalSickDay
        {
            get
            {
                return this.StaffVacations.FirstOrDefault(t => t.Type == VacationType.SickDay)?.Total;
            }
        }

        /// <summary>
        /// 可用调休假
        /// </summary>
        public decimal? TotalOffDay
        {
            get
            {
                return this.StaffVacations.FirstOrDefault(t => t.Type == VacationType.OffDay)?.Lefts;
            }
        }

        /// <summary>
        /// 已请年假天数
        /// </summary>
        public decimal UsedYearDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.AnnualLeave && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.AnnualLeave && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }

        /// <summary>
        /// 已请调休假
        /// </summary>
        public decimal UsedOffDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.LeaveInLieu && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.LeaveInLieu && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }

        /// <summary>
        /// 已请病假天数
        /// </summary>
        public decimal UsedSickDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.SickLeave && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.SickLeave && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }

        /// <summary>
        /// 已请事假天数
        /// </summary>
        public decimal CasualLeaveDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.CasualLeave && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.CasualLeave && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }

        /// <summary>
        /// 已请公务天数
        /// </summary>
        public decimal OfficialBusinessDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.OfficialBusiness && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.OfficialBusiness && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }

        /// <summary>
        /// 已请公差天数
        /// </summary>
        public decimal BusinessTripDays
        {
            get
            {
                decimal day1 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.BusinessTrip && item.Date.Type == DateLengthType.AllDay).Count();
                decimal day2 = this.OffTimeDates.Where(item => item.LeaveType == Underly.Enums.LeaveType.BusinessTrip && item.Date.Type != DateLengthType.AllDay).Count();
                return day1 + day2 * 0.5m;
            }
        }
    }

    public class ApplicationContent
    {
        public string ApplicaitonID { get; set; }

        public OffTimeContent Content { get; set; }
    }

    public class OffTimeContent
    {
        /// <summary>
        /// 请假类型
        /// </summary>
        public Underly.Enums.LeaveType LeaveType { get; set; }

        /// <summary>
        /// 请假日期项
        /// </summary>
        public List<OffTimeDateItem> DateItems { get; set; }
    }

    public class OffTimeDate
    {
        public string ApplicaitonID { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        public Underly.Enums.LeaveType LeaveType { get; set; }

        /// <summary>
        /// 请假日期
        /// </summary>
        public OffTimeDateItem Date { get; set; }
    }
}