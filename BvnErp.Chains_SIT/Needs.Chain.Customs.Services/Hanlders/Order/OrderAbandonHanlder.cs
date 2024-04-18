using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.Deleted 事件
    ///  订单取消
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderAbandonHanlder(object sender, OrderAbandonEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderAbandonEventArgs : EventArgs
    {
        public Models.DraftOrder Order { get; private set; }

        public OrderAbandonEventArgs(Models.DraftOrder order)
        {
            this.Order = order;
        }

        public OrderAbandonEventArgs()
        {

        }
    }
}