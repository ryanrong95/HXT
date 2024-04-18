using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class TaxCategoryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.TaxCategories ToLinq(this Models.TaxCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.TaxCategories
            {
                ID = entity.ID,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                KeyWords = entity.KeyWords,
                Description = entity.KeyWords,
                IsElectronic = entity.IsElectronic
            };
        }
    }
}
