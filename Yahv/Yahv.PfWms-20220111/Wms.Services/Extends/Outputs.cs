using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Outputs
    {
        public static Layers.Data.Sqls.PvWms.Outputs ToLinq(this Models.Output entity)
        {
            return new Layers.Data.Sqls.PvWms.Outputs
            {
                ID = entity.ID,
                Currency = (int)entity.Currency,
                CustomerServiceID = entity.CustomerServiceID,
                ItemID = entity.ItemID,
                CreateDate=entity.CreateDate,
                InputID=entity.InputID,
                Price=entity.Price,
                OrderID=entity.OrderID,
                OwnerID=entity.OwnerID,
                PurchaserID=entity.PurchaserID,
                SalerID=entity.SalerID

        
            };
        }
    }
}
