using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产地变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">状态改变事件参数</param>
    public delegate void SplitModelChangedHanlder(object sender, SplitModelChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class SplitModelChangedEventArgs : EventArgs
    {
        public dynamic Object { get; private set; }

        public string Origin { get; private set; }
        public decimal Qty { get; private set; }
        public string Manufature { get; private set; }
        public Admin Admin { get; private set; }
        public  decimal OldTotalPrice { get; private set; }

        public string NewOrderItemID { get; set; }

        public SplitModelChangedEventArgs(object entity, string origin, decimal qty, string manufature, Admin admin, decimal OldTotalPrice, string newOrderItemID)
        {
            this.Object = entity;
            this.Origin = origin;
            this.Qty = qty;
            this.Manufature = manufature;
            this.Admin = admin;
            this.OldTotalPrice = OldTotalPrice;
            this.NewOrderItemID = newOrderItemID;
        }

        public SplitModelChangedEventArgs() { }
    }
}