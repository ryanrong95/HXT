using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class ClientProductTaxCategoryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories ToLinq(this Models.ClientProductTaxCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories
            {
                ID = entity.ID,
                ClientID = entity.Client.ID,
                Name = entity.Name,
                Model = entity.Model,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                Status = (int)entity.Status,
                TaxStatus=(int)entity.TaxStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}