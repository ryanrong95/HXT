using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models.Orders
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 创建中
        /// </summary>
        /// <example>
        /// 时间可能会特别短
        /// </example>
        Creating = 0,
        /// <summary>
        /// 已下单
        /// </summary>
        Placed = 1,
        /// <summary>
        /// 等待支付
        /// </summary>
        Paying = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 600,
        /// <summary>
        /// 已关闭
        /// </summary>
        Closed = 700
    }

    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 期货
        /// </summary>
        Future = 2,
    }

    /// <summary>
    /// 承运方式
    /// </summary>
    public enum TransportTerm
    {
        /// <summary>
        /// 自提
        /// </summary>
        SelfPickUp = 1,
        /// <summary>
        /// UPS
        /// </summary>
        UPS = 2,
        /// <summary>
        /// FedEx
        /// </summary>
        FedEx = 3,
        /// <summary>
        /// DHL
        /// </summary>
        DHL = 4,
        /// <summary>
        /// 顺丰
        /// </summary>
        SF = 5,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 6,
    }
}
