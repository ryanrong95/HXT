using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views
{
    public class LsOrderItemRoll : LsOrderItemsAll
    {
        string OrderID;
        public LsOrderItemRoll(string orderid)
        {
            this.OrderID = orderid;
        }
        protected override IQueryable<LsOrderItem> GetIQueryable()
        {
            return base.GetIQueryable().Where(item=>item.OrderID==this.OrderID);
        }
    }
}
