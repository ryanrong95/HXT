using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 结算方式
    /// </summary>
    public enum ClearType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        Monthly = 1,
        /// <summary>
        /// 发票后
        /// </summary>
        [Description("发票后")]
        AfterInvoice = 2,
        /// <summary>
        /// 到货后
        /// </summary>
        [Description("到货后")]
        AfterDelivery = 3
    }

}
