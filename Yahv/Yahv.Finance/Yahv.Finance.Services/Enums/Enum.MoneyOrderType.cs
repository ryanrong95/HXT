using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 承兑汇票类型
    /// </summary>
    public enum MoneyOrderType
    {
        /// <summary>
        /// 银行承兑
        /// </summary>
        [Description("银行承兑")]
        Bank = 1,

        /// <summary>
        /// 商业承兑
        /// </summary>
        [Description("商业承兑")]
        Commercial = 2,
    }
}