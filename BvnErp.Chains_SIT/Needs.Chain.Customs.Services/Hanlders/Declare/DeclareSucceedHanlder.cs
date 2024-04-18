using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 报关成功委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DeclareSucceedHanlder(object sender, DeclareSucceedEventArgs e);

    /// <summary>
    /// 报关成功事件参数参数
    /// </summary>
    public class DeclareSucceedEventArgs : EventArgs
    {
        public Models.DecHead DecHead { get; private set; }

        public Models.QuoteConfirmedOrder Order { get; private set; }

        public DeclareSucceedEventArgs(Models.DecHead entity, Models.QuoteConfirmedOrder Order)
        {
            this.DecHead = entity;
            this.Order = Order;
        }

        public DeclareSucceedEventArgs() { }
    }
}
