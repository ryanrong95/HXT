using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services.Enums
{
    /// <summary>
    /// 开票类型
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 全额发票
        /// </summary>
        [Description("全额发票")]
        Full = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        [Description("服务费发票")]
        Service = 1,
    }

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

    /// <summary>
    /// 开票通知状态
    /// </summary>
    public enum InvoiceNoticeStatus
    {
        [Description("待开票")]
        UnAudit = 1,

        [Description("开票中")]
        Auditing = 2,

        [Description("已完成")]
        Confirmed = 3,

        [Description("已取消")]
        Canceled = 4
    }

}
