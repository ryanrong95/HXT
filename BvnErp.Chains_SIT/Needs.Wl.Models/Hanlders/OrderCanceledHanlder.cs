using System;

namespace Needs.Wl.Models.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.Cancele 事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderCanceledHanlder(object sender, OrderCanceledEventArgs e);

    /// <summary>
    /// 订单取消事件参数
    /// </summary>
    public class OrderCanceledEventArgs : EventArgs
    {
        public Models.Order Order { get; private set; }

        public OrderCanceledEventArgs(Models.Order order)
        {
            this.Order = order;
        }

        public OrderCanceledEventArgs() { }
    }
}