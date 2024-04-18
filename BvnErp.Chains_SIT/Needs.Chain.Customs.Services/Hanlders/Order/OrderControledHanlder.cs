using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    ///  订单管控事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderControledHanlder(object sender, OrderControledEventArgs e);

    /// <summary>
    /// 订单管控事件参数
    /// </summary>
    public class OrderControledEventArgs : EventArgs
    {
        public Models.OrderControlBase OrderControl { get; private set; }

        public OrderControledEventArgs(Models.OrderControlBase orderControl)
        {
            this.OrderControl = orderControl;
        }

        public OrderControledEventArgs()
        {
        }
    }
}
