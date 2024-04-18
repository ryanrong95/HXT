using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 提交拆分结果给董健
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void OrderSplitSubmitHanlder(object sender, OrderSplitSubmitEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderSplitSubmitEventArgs : EventArgs
    {
        public SplitOrder Order { get; set; }
      
        public Admin Operator { get; set; }

        public OrderSplitSubmitEventArgs(Admin admin, SplitOrder myOrder)
        {           
            this.Operator = admin;           
            this.Order = myOrder;
        }

        public OrderSplitSubmitEventArgs()
        {

        }
    }
}