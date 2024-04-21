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
    public class WayCostsView : UniqueView<WayCosts, PvWmsRepository>
    {
        protected override IQueryable<WayCosts> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WayCosts>()
                   select new WayCosts
                   {
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       Name = entity.Name,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID
                   };
        }
    }
}
