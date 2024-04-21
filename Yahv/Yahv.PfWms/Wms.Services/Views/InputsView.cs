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
    public class InputsView : UniqueView<Models.Inputs, PvWmsRepository>
    {
        public InputsView()
        {

        }
        protected override IQueryable<Inputs> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Inputs>()
                   select new Models.Inputs
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       ItemID = entity.ItemID,
                       OrderID = entity.OrderID,
                       TinyOrderID=entity.TinyOrderID,
                       OriginID = entity.OriginID,
                       PurchaserID = entity.PurchaserID ,
                       SalerID = entity.SalerID,
                       Currency=(Currency)entity.Currency,
                       ProductID=entity.ProductID,
                       CreateDate=entity.CreateDate,
                       UnitPrice=entity.UnitPrice,
                       ClientID=entity.ClientID,
                       TrackerID=entity.TrackerID
                       ,PayeeID=entity.PayeeID,
                       ThirdID=entity.PayeeID
                  
                   };
        }
    }
}
