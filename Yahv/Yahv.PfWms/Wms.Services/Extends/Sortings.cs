using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Sortings
    {
        public static Layers.Data.Sqls.PvWms.Sortings ToLinq(this Models.Sorting entity)
        {
            return new Layers.Data.Sqls.PvWms.Sortings
            {
                ID = entity.ID,
                AdminID = entity.AdminID,
                BoxCode = entity.BoxCode,
                CreateDate = entity.CreateDate,
                NoticeID = entity.NoticeID,
                Quantity = entity.Quantity,
                Weight = entity.Weight,
                Volume=entity.Volume
            };
        }
    }
}
