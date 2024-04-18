using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        [Description("等待处理")]
        Waiting = 100,

        /// <summary>
        /// 已入库
        /// </summary>
        [Description("已入库")]
        Storaged = 200,

        /// <summary>
        /// 拒收
        /// </summary>
        [Description("拒收")]
        Rejected = 401,

        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Closed = 500,

        /// <summary>
        /// 处理完成
        /// </summary>
        [Description("处理完成")]
        Completed = 600,
    }
}
