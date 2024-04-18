using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 1,

        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 2,

        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        TransferAccount = 3,
    }
}