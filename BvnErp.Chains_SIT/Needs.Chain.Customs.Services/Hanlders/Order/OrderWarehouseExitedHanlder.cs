using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.OrderWarehouseExited 事件
    ///  订单出库完成
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderWarehouseExitedHanlder(object sender, OrderWarehouseExitedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderWarehouseExitedEventArgs : EventArgs
    {
        public Models.DeclaredOrder Order { get; private set; }

        public string OrderID { get; private set; }

        public OrderWarehouseExitedEventArgs(Models.DeclaredOrder order)
        {
            this.Order = order;
        }

        public OrderWarehouseExitedEventArgs(string orderID)
        {
            this.OrderID = orderID;
        }

        public OrderWarehouseExitedEventArgs() { }
    }
}
