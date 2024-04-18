using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 返款方式
    /// </summary>
    public enum CommissionMethod
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 抵扣货款
        /// </summary>
        [Description("抵扣货款")]
        Deduction = 1,
        /// <summary>
        /// 抵扣货款
        /// </summary>
        [Description("单独结算")]
        SeparateSettlement = 2,
    }
}
