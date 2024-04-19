using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Models.Orders.Commodity;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class CommodityOutputsView : QueryView<CommodityOutput, Layer.Data.Sqls.BvOrdersReponsitory>
    {

        public CommodityOutputsView()
        {
        }


        protected override IQueryable<CommodityOutput> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.CommodityOutputs>()
                        select new CommodityOutput
                        {
                            ID = entity.ID,
                            OrderID = entity.OrderID,
                            InputID = entity.InputID,
                            ServiceInputID = entity.ServiceInputID,
                            Count = entity.Count,
                            CreateDate = entity.CreateDate
                        };
            return linqs;
        }
    }
}
