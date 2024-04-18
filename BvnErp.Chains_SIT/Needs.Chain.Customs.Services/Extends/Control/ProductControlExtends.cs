using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class ProductControlExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ProductControls ToLinq(this Models.ProductControl entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductControls
            {
                ID = entity.ID,
                Name = entity.Name,
                Model = entity.Model,
                Manufacturer = entity.Manufacturer,
                Type = (int)entity.Type,
                CreateDate = entity.CreateDate
            };
        }
    }
}