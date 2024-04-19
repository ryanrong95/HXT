using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Orders
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
        [Naming("创建中")]
        Creating = 0,
        /// <summary>
        /// 已下单
        /// </summary>
        [Naming("已下单")]
        Placed = 1,
        /// <summary>
        /// 等待支付
        /// </summary>
        [Naming("等待支付")]
        Paying = 1,
        /// <summary>
        /// 已完成
        /// </summary>
        [Naming("已完成")]
        Completed = 600,
        /// <summary>
        /// 已关闭
        /// </summary>
        [Naming("已关闭")]
        Closed = 700
    }

    /// <summary>
    /// 订单来源
    /// </summary>
    [Flags]
    public enum Source
    {
        /// <summary>
        /// 线上
        /// </summary>
        [Naming("线上")]
        Online = 1,
        /// <summary>
        /// 线下
        /// </summary>
        [Naming("线下")]
        Offline = 2,
        /// <summary>
        /// 正常
        /// </summary>
        [Naming("正常")]
        Normal = 4,
        /// <summary>
        /// 期货
        /// </summary>
        [Naming("期货")]
        Future = 8,
    }

    /// <summary>
    /// 运费支付
    /// </summary>
    public enum FreightPayer
    {
        /// <summary>
        /// 收货人
        /// </summary>
        [Naming("收货人")]
        Consignee,
        /// <summary>
        /// 发货人
        /// </summary>
        [Naming("发货人")]
        Consignor
    }


    /// <summary>
    /// 承运方式
    /// </summary>
    public enum TransportTerm
    {
        /// <summary>
        /// 自提
        /// </summary>
        [Naming("SelfPickUp")]
        SelfPickUp = 1,
        /// <summary>
        /// UPS
        /// </summary>
        [Naming("UPS")]
        UPS = 2,
        /// <summary>
        /// FedEx
        /// </summary>
        [Naming("FedEx")]
        FedEx = 3,
        /// <summary>
        /// DHL
        /// </summary>
        [Naming("DHL")]
        DHL = 4,
        /// <summary>
        /// 顺丰
        /// </summary>
        [Naming("SF")]
        SF = 5,
        /// <summary>
        /// 其他
        /// </summary>
        [Naming("其他")]
        Other = 6,
    }
}
