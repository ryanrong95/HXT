using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Forms
    {
        public static Layers.Data.Sqls.PvWms.Forms ToLinq(this Models.Form entity)
        {
            return new Layers.Data.Sqls.PvWms.Forms
            {
                ID = entity.ID,
                StorageID=entity.StorageID,
                Quantity=entity.Quantity,
                NoticeID = entity.NoticeID,
                Status=(int)entity.Status,
            };
        }
    }
}
