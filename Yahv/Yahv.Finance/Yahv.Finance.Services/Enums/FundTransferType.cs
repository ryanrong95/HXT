using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 调拨类型
    /// </summary>
    public enum FundTransferType
    {
        /// <summary>
        /// 借款
        /// </summary>
        [Description("借款")]
        Loan = 1,
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        ProductFee = 2,
        /// <summary>
        /// 调账
        /// </summary>
        [Description("调账")]
        Transfer = 3,
        /// <summary>
        /// 购汇
        /// </summary>
        [Description("购汇")]
        ForeignExchangePurchasing = 4,
        /// <summary>
        /// 还款
        /// </summary>
        [Description("还款")]
        Repayment = 5,
        /// <summary>
        /// 贴现
        /// </summary>
        [Description("贴现")]
        Discount = 6,
        /// <summary>
        /// 资金往来
        /// </summary>
        [Description("资金往来")]
        Transaction = 7,
        /// <summary>
        /// 到期存入
        /// </summary>
        [Description("到期存入")]
        DueEnter = 8,
    }
}