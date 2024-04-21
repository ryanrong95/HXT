using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 员工假期类型
    /// </summary>
    public enum VacationType
    {
        [Description("年假")]
        YearsDay = 1,

        [Description("调休假")]
        OffDay = 2,

        [Description("病假")]
        SickDay = 3,

        [Description("产检假")]
        ProductionInspectionDay = 4,
    }

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
