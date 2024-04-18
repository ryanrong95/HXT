using Needs.Ccs.Services.Models;
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
    public delegate void OrderSplitHanlder(object sender, OrderSplitEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class OrderSplitEventArgs : EventArgs
    {
        public Order Order { get; set; }
        public string OldOrderID { get; set; }
        //public string EntryNoticeID { get; private set; }
        public Admin Operator { get; set; }

        public OrderSplitEventArgs(Admin admin,string oldOrderID,Order myOrder)
        {
            //this.EntryNoticeID = entryNoticeID;
            this.Operator = admin;
            this.OldOrderID = oldOrderID;
            this.Order = myOrder;
        }

        public OrderSplitEventArgs()
        {

        }
    }
}