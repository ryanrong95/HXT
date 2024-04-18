using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class ProductCategoryTaxExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ProductTaxCategories ToLinq(this Models.ProductTaxCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductTaxCategories
            {
                ID = entity.ID,
                Model = entity.Model,
                Name = entity.Name,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                CreateTime = entity.CreateDate
            };
        }
    }
}
