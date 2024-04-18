using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views
{
    public class OrderStorageRoll : StoragesAlls
    {
        string OrderID = string.Empty;

        public OrderStorageRoll(string orderID)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.OrderID == this.OrderID);
            return linq;
        }
    }
}
