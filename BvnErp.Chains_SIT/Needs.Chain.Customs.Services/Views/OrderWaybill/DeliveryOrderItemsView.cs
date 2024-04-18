using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DeliveryOrderItemsView : UniqueView<Models.DeliveryOrderItem, ScCustomsReponsitory>
    {
        public DeliveryOrderItemsView()
        {
        }

        public DeliveryOrderItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DeliveryOrderItem> GetIQueryable()
        {
            var linq = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSZOutputDetail>()
                       join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on c.ItemID equals d.ID
                       select new DeliveryOrderItem
                       {
                           DeliveryOrderID = c.WaybillID,
                           Model = c.PartNumber,
                           Brand = c.Manufacturer,
                           Qty = c.Quantity,
                           Name = d.Name
                       };

            return linq;
        }
    }
}
