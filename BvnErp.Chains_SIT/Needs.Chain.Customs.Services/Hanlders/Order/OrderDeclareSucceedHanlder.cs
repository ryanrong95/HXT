using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  表示用于处理的方法 Order.OrderDeclareSucceed 事件
    ///  订单报关成功
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderDeclareSucceedHanlder(object sender, OrderDeclareSucceedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderDeclareSucceedEventArgs : EventArgs
    {
        public Models.QuoteConfirmedOrder Order { get; private set; }

        public OrderDeclareSucceedEventArgs(Models.QuoteConfirmedOrder order)
        {
            this.Order = order;
        }

        public OrderDeclareSucceedEventArgs() { }
    }
}