using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 表示用于处理的方法 Order.HangUp 事件
    /// 订单挂起
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">订单挂起事件参数</param>
    public delegate void OrderHangUpedHanlder(object sender, OrderHangUpedEventArgs e);

    /// <summary>
    /// 订单挂起事件参数
    /// </summary>
    public class OrderHangUpedEventArgs : EventArgs
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单ItemID
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 订单挂起类型
        /// </summary>
        public Enums.OrderControlType OrderControlType { get; set; }

        /// <summary>
        /// 订单审核步骤/审核层级
        /// </summary>
        public Enums.OrderControlStep OrderControlStep { get; set; }

        /// <summary>
        /// 挂起原因
        /// </summary>
        public string Summary { get; set; }

        public OrderHangUpedEventArgs(string orderID, Enums.OrderControlType orderControlType, string Summary)
        {
            this.OrderID = orderID;
            this.OrderControlType = orderControlType;
            this.Summary = Summary;
        }

        public OrderHangUpedEventArgs(string orderID, string orderItemID, Enums.OrderControlType orderControlType,
                                     Enums.OrderControlStep orderControlStep, string summary)
        {
            this.OrderID = orderID;
            this.OrderItemID = orderItemID;
            this.OrderControlType = orderControlType;
            this.OrderControlStep = orderControlStep;
            this.Summary = summary;
        }
    }
}