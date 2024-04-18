using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    /// <summary>
    /// 账期类型
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        PrePaid = 0,

        /// <summary>
        /// 约定期限
        /// </summary>
        [Description("约定期限")]
        AgreedPeriod = 1,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        Monthly = 2,
    }
}