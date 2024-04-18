using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 发票交付方式
    /// </summary>
    public enum InvoiceDeliveryType
    {
        /// <summary>
        /// 邮寄
        /// </summary>
        [Description("邮寄")]
        SendByPost = 1,

        /// <summary>
        /// 随货同行
        /// </summary>
        [Description("随货同行")]
        FollowUpGoods = 2,
    }
}
