using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Specs
    {
        public static Layers.Data.Sqls.PvWms.Specs ToLinq(this Models.Specs entity)
        {
            return new Layers.Data.Sqls.PvWms.Specs
            {
                ID = entity.ID.ToUpper(),
                Name = entity.Name,
                //Type = (int)entity.Type,
                Width = entity.Width,
                Length = entity.Length,
                Height = entity.Height,
                //RowTotal = entity.RowTotal,
                //Volume = entity.Volume,
                Load = entity.Load,
            };
        }

    }
}
