using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Underly;

namespace Wms.Services.Views
{
    public class OutputsView : UniqueView<Models.Output, PvWmsRepository>
    {
        public OutputsView()
        {
        }
        protected override IQueryable<Output> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Outputs>()
                   select new Models.Output
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       InputID = entity.InputID,
                       Currency =(Currency) entity.Currency,
                       CustomerServiceID=entity.CustomerServiceID,
                       ItemID=entity.ItemID,
                       Price=entity.Price,
                       OrderID=entity.OrderID,
                       OwnerID=entity.OwnerID,
                       PurchaserID=entity.PurchaserID,
                       SalerID=entity.SalerID


                   };
        }
    }
}
