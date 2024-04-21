using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 承兑汇票性质
    /// </summary>
    public enum MoneyOrderNature
    {
        /// <summary>
        /// 电子版
        /// </summary>
        [Description("电子版")]
        Electronic = 1,
        /// <summary>
        /// 纸质版
        /// </summary>
        [Description("纸质版")]
        Payper = 2
    }
}