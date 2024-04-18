using System;

namespace Needs.Wl.Models.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.Deleted 事件
    ///  订单取消
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderDeletedHanlder(object sender, OrderDeletedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderDeletedEventArgs : EventArgs
    {
        public Models.Order Order { get; private set; }

        public OrderDeletedEventArgs(Models.Order order)
        {
            this.Order = order;
        }

        public OrderDeletedEventArgs()
        {

        }
    }
}