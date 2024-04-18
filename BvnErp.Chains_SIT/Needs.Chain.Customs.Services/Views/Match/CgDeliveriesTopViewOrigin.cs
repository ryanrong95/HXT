using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CgDeliveriesTopViewOrigin : UniqueView<Models.CgDeliveriesTopViewModel, ScCustomsReponsitory>
    {
        public CgDeliveriesTopViewOrigin()
        {
        }

        internal CgDeliveriesTopViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CgDeliveriesTopViewModel> GetIQueryable()
        {
            var data = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgDeliveriesTopView>()
                       select new Models.CgDeliveriesTopViewModel
                       {
                           ID = c.InputID,
                           CaseNo = c.PackageCase,
                           OrderItemID = c.ItemID,
                           Manufacturer = c.Manufacturer,
                           Model = c.PartNumber,
                           Origin = c.Origin,
                           Quantity = c.Quantity,
                           MainOrderID = c.OrderID,
                           OrderID = c.TinyOrderID,
                           Batch = c.DateCode,
                           Type = c.Type
                       };

            return data;
        }
    }
}
