using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.AttendanceData.Import
{
    /*
    /// <summary>
    /// 实际考勤情况
    /// </summary>
    public enum AttendInFact
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

        [Description("产假")]
        MaternityLeave = 6,

        [Description("陪产假")]
        PaternityLeave = 7,

        [Description("婚假")]
        MarriageLeave = 8,

        [Description("丧假")]
        FuneralLeave = 9,

        [Description("公务")]
        OfficialBusiness = 10,

        [Description("加班")]
        Overtime = 11,

        [Description("公休日")]
        PublicHoliday = 12,

        [Description("法定节假日")]
        LegalHolidays = 13,

        [Description("系统授权")]
        SystemAuthorizing = 14,
    }
    */

    /// <summary>
    /// 加班兑换方式
    /// </summary>
    public enum OvertimeExchangeType
    {
        [Description("调休假")]
        PayDay = 1,

        [Description("加班费")]
        PayMoney = 2,
    }
}
