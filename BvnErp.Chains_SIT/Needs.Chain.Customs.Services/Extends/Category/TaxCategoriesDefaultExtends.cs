using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class TaxCategoriesDefaultExtends
    {
        public static Layer.Data.Sqls.ScCustoms.TaxCategoriesDefaults ToLinq(this Models.TaxCategoriesDefault entity)
        {
            return new Layer.Data.Sqls.ScCustoms.TaxCategoriesDefaults
            {
                ID = entity.ID,
                TaxCode = entity.TaxCode,
                TaxFirstCategory = entity.TaxFirstCategory,
                TaxSecondCategory = entity.TaxSecondCategory,
                TaxThirdCategory = entity.TaxThirdCategory,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
