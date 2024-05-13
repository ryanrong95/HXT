using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class WayChargos
    {
        public static Layers.Data.Sqls.PvWms.WayChargos ToLinq(this Models.WayChargos entity)
        {
            return new Layers.Data.Sqls.PvWms.WayChargos
            {
                ID=entity.ID,
                Payer =(int)entity.Payer,
                PayMethod = (int)entity.PayMethod,
                Currency = (int)entity.Currency,
                TotalPrice = entity.TotalPrice,
            };
        }
    }
}
