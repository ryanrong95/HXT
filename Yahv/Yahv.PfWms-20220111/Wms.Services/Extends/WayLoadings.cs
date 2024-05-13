using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class WayLoadings
    {
        public static Layers.Data.Sqls.PvWms.WayLoadings ToLinq(this Models.WayLoadings entity)
        {
            return new Layers.Data.Sqls.PvWms.WayLoadings
            {
                ID=entity.ID,
                TakingDate = entity.TakingDate,
                TakingAddress = entity.TakingAddress,
                TakingContact = entity.TakingContact,
                TakingPhone = entity.TakingPhone,
                CarNumber1 = entity.CarNumber1,
                Driver = entity.Driver,
                Carload = entity.Carload,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                CreatorID = entity.CreatorID,
                ModifierID = entity.ModifierID,
            };
        }
    }
}
