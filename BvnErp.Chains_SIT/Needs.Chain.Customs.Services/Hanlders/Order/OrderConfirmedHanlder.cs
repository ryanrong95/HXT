using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.Confirm 事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderConfirmedHanlder(object sender, OrderConfirmedEventArgs e);

    /// <summary>
    ///  Order.Confirm 事件参数
    /// </summary>
    public class OrderConfirmedEventArgs : EventArgs
    {
        public Models.Order Order { get; private set; }

        public OrderConfirmedEventArgs(Models.Order order)
        {
            this.Order = order;
        }

        public OrderConfirmedEventArgs()
        {
        }
    }
}