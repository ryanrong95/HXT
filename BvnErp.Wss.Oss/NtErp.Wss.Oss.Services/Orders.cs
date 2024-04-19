using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 现货
        /// </summary>
        [Description("现货")]
        Normal = 1,
        /// <summary>
        /// 期货
        /// </summary>
        [Description("期货")]
        Future = 2
    }
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 等待支付
        /// </summary>
        [Description("等待支付")]
        Paying = 1,
        /// <summary>
        /// 已支付
        /// </summary>
        [Description("已支付")]
        HasPaid = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 200,
        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        Closed = 400
    }
}
