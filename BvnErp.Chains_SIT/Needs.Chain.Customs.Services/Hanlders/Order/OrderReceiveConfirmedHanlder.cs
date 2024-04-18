using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 表示用于处理的方法 Order.HangUpCanceled 事件
    /// 订单取消挂起
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void OrderReceiveConfirmedHanlder(object sender, OrderReceiveConfirmedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderReceiveConfirmedEventArgs : EventArgs
    {
        public Models.Order Order { get; private set; }

        public OrderReceiveConfirmedEventArgs(Models.Order order)
        {
            this.Order = order;
        }

        public OrderReceiveConfirmedEventArgs() { }
    }
}
