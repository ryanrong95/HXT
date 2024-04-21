using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Warehouses
    {
        public static Layers.Data.Sqls.PvWms.Warehouses ToLinq(this Models.Warehouse entity)
        {
            return new Layers.Data.Sqls.PvWms.Warehouses
            {
                ID = entity.ID.ToUpper(),
                Name = entity.Name,
                Address = entity.Address,
                CrmCode=entity.CrmCode,
                IsOnOrder=entity.IsOnOrder 

            };
        }

    }
}
