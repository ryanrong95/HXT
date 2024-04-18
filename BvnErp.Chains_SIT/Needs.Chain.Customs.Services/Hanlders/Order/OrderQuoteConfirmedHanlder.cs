using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 订单报价确认
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderQuoteConfirmedHanlder(object sender, OrderQuoteConfirmedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderQuoteConfirmedEventArgs : EventArgs
    {
        public Models.QuotedOrder Order { get; private set; }

        public string OrderID { get; private set; }

        public OrderQuoteConfirmedEventArgs(Models.QuotedOrder order)
        {
            this.Order = order;
        }

        public OrderQuoteConfirmedEventArgs(string orderID)
        {
            this.OrderID = orderID;
        }

        public OrderQuoteConfirmedEventArgs() { }
    }
}
