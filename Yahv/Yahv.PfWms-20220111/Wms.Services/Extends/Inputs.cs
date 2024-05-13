using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Inputs
    {
        public static Layers.Data.Sqls.PvWms.Inputs ToLinq(this Models.Inputs entity)
        {
            return new Layers.Data.Sqls.PvWms.Inputs
            {
                ID = entity.ID,
                Code = entity.Code,
                Currency = (int)entity.Currency,
                TrackerID = entity.TrackerID,
                ItemID = entity.ItemID,
                OrderID = entity.OrderID,
                TinyOrderID=entity.TinyOrderID,
                CreateDate=entity.CreateDate,
                ClientID=entity.ClientID,
                PayeeID=entity.PayeeID,
                ThirdID=entity.ThirdID,
                OriginID=entity.OriginID,
                
                ProductID = entity.ProductID,
                PurchaserID = entity.PurchaserID,
                SalerID = entity.SalerID,
                //Supplier = entity.Supplier,
                UnitPrice = entity.UnitPrice,
            };
        }
    }
}
