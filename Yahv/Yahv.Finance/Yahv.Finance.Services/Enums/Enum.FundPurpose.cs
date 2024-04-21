using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 资金用途
    /// </summary>
    public enum FundPurpose
    {
        /// <summary>
        /// 购汇
        /// </summary>
        [Description("购汇")]
        ForeignExchangePurchasing = 1,

        /// <summary>
        /// 借款
        /// </summary>
        [Description("借款")]
        Loan = 2,

        /// <summary>
        /// 还款
        /// </summary>
        [Description("还款")]
        Repayment = 3,

        /// <summary>
        /// 付货款
        /// </summary>
        [Description("付货款")]
        PayGoods =4,

        /// <summary>
        /// 贴现
        /// </summary>
        [Description("贴现")]
        Discount =5,
    }
}