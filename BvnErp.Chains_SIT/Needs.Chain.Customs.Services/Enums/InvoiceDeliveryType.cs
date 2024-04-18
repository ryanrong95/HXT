using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
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

        /// <summary>
        /// 自取
        /// </summary>
        [Description("自取")]
        TakeYourOwn = 3,
    }
}
