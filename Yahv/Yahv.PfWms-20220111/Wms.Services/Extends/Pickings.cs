using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Pickings
    {
        public static Layers.Data.Sqls.PvWms.Pickings ToLinq(this Models.Pickings entity)
        {
            return new Layers.Data.Sqls.PvWms.Pickings
            {
                ID = entity.ID,
                AdminID = entity.AdminID,
                BoxCode = entity.BoxingCode,
                CreateDate = entity.CreateDate,
                NoticeID = entity.NoticeID,
                Quantity = entity.Quantity,
                Volume = entity.Volume,
                Weight = entity.Weight,
                NetWeight=entity.NetWeight,
                StorageID=entity.StorageID
            };
        }
    }
}
