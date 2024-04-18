using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 个人日程安排类型
    /// </summary>
    public enum SchedulePrivateType
    {
        [Description("公务")]
        OfficialBusiness = 1,

        [Description("公差")]
        BusinessTrip = 2,

        [Description("年假")]
        AnnualLeave = 3,

        [Description("事假")]
        CasualLeave = 4,

        [Description("病假")]
        SickLeave = 5,

        [Description("调休")]
        LeaveInLieu = 6,

        [Description("婚假")]
        MarriageLeave = 7,

        [Description("产检假")]
        ProductionInspectionLeave = 8,

        [Description("陪产假")]
        PaternityLeave = 9,

        [Description("产假")]
        MaternityLeave = 10,

        [Description("丧假")]
        FuneralLeave = 11,

        [Description("加班")]
        Overtime = 12,

        [Description("补签")]
        ReSign = 13,

        [Description("育儿假")]
        ParentalLeave = 14,
    }

    /// <summary>
    /// 上午下午
    /// </summary>
    public enum AmOrPm
    {
        [Description("上午")]
        Am = 1,

        [Description("下午")]
        Pm = 2,
    }

    /// <summary>
    /// 请假类型
    /// </summary>
    public enum LeaveType
    {
        [Description("公务")]
        OfficialBusiness = 1,

        [Description("公差")]
        BusinessTrip = 2,

        [Description("年假")]
        AnnualLeave = 3,

        [Description("事假")]
        CasualLeave = 4,

        [Description("病假")]
        SickLeave = 5,

        [Description("调休")]
        LeaveInLieu = 6,

        [Description("婚假")]
        MarriageLeave = 7,

        [Description("产检假")]
        ProductionInspectionLeave = 8,

        [Description("陪产假")]
        PaternityLeave = 9,

        [Description("产假")]
        MaternityLeave = 10,

        [Description("丧假")]
        FuneralLeave = 11,

        [Description("育儿假")]
        ParentalLeave = 14,
    }

    /// <summary>
    /// 实际考勤情况
    /// </summary>
    public enum AttendInFactType
    {
        [Description("正常")]
        Normal = 1,

        [Description("旷工")]
        Absenteeism = 2,

        [Description("事假")]
        CasualLeave = 3,

        [Description("病假")]
        SickLeave = 4,

        [Description("带薪休假")]
        PaidLeave = 5,

        [Description("公务")]
        OfficialBusiness = 6,

        [Description("加班")]
        Overtime = 7,

        [Description("公休日")]
        PublicHoliday = 8,

        [Description("法定节假日")]
        LegalHolidays = 9,

        [Description("公差")]
        BusinessTrip = 10,

        [Description("系统授权")]
        SystemAuthorizing = 11,
    }
}
