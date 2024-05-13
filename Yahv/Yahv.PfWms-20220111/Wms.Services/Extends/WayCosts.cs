using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class WayCosts
    {
        public static Layers.Data.Sqls.PvWms.WayCosts ToLinq(this Models.WayCosts entity)
        {
            return new Layers.Data.Sqls.PvWms.WayCosts
            {
                ID=entity.ID,
                WaybillID = entity.WaybillID,
                Name = entity.Name,
                Currency =(int) entity.Currency,
                Price = entity.Price,
                CreateDate = entity.CreateDate,
                CreatorID = entity.CreatorID
            };
        }
    }
}
