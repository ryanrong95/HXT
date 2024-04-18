using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 发票交付方式
    /// </summary>
    public enum InvoiceDeliveryType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,

        /// <summary>
        /// 单独邮寄
        /// </summary>
        [Description("单独邮寄")]
        SendByPost = 1,

        /// <summary>
        /// 随货同行
        /// </summary>
        [Description("随货同行")]
        FollowUpGoods = 2,

        /// <summary>
        /// 自取
        /// </summary>
        [Description("自取")]
        HelpYourself = 3,
    }
}
