using System;

namespace Needs.Wl.Models
{
    public static class ClientProductTaxCategoryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories ToLinq(this Models.ClientProductTaxCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Name = entity.Name,
                Model = entity.Model,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                Status = (int)entity.Status,
                TaxStatus = (int)entity.TaxStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}