using Yahv.Underly.Attributes;

namespace Yahv.Payments
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PayMethodType
    {
        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Unknown = 0,

        /// <summary>
        /// 信用花费
        /// </summary>
        [Description("信用花费")]
        CreditCost = 10,

        /// <summary>
        /// 现金（包括银行）
        /// </summary>
        [Description("现金")]
        Cash = 20,
    }
}