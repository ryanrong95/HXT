using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 来源
    /// </summary>
    public enum OrderItemFrom
    {
        /// <summary>
        /// 线上
        /// </summary>
        [Description("线上")]
        Cart = 1,
        /// <summary>
        /// 后台加入购物车项
        /// </summary>
        [Description("虚拟产品")]
        VirtualCart = 2,
        /// <summary>
        /// 后台加入订单项
        /// </summary>
        [Description("虚拟订单项")]
        VirtualItem = 3
    }
}
