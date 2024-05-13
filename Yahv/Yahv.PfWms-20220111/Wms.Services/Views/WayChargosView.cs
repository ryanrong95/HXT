using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Wms.Services.Views
{
    public class WayChargosView : UniqueView<Models.WayChargos, PvWmsRepository>
    {
        protected override IQueryable<Models.WayChargos> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WayChargos>()
                   select new Models.WayChargos
                   {
                       ID = entity.ID,
                       Payer = (Enums.Payer)entity.Payer,
                       PayMethod = (Enums.PayMethod)entity.PayMethod,
                       Currency = (Currency)entity.Currency,
                       TotalPrice = entity.TotalPrice,
                   };
        }
    }
}
