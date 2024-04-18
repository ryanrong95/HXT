using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class PreProductCategoriesOrigin : UniqueView<Models.PreClassifyProduct, ScCustomsReponsitory>
    {
        internal PreProductCategoriesOrigin()
        {
        }

        internal PreProductCategoriesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PreClassifyProduct> GetIQueryable()
        {
            return from preclassifyproduct in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
                   select new Models.PreClassifyProduct
                   {
                       ID = preclassifyproduct.ID,
                       PreProductID = preclassifyproduct.PreProductID,
                       Model = preclassifyproduct.Model,
                       Manufacturer = preclassifyproduct.Manufacture,
                       ProductName = preclassifyproduct.ProductName,
                       HSCode = preclassifyproduct.HSCode,
                       TariffRate = preclassifyproduct.TariffRate,
                       AddedValueRate = preclassifyproduct.AddedValueRate,
                       ExciseTaxRate = preclassifyproduct.ExciseTaxRate,
                       TaxCode = preclassifyproduct.TaxCode,
                       TaxName = preclassifyproduct.TaxName,
                       Type = (Enums.ItemCategoryType?)preclassifyproduct.Type,
                       InspectionFee = preclassifyproduct.InspectionFee,
                       Unit1 = preclassifyproduct.Unit1,
                       Unit2 = preclassifyproduct.Unit2,
                       CIQCode = preclassifyproduct.CIQCode,
                       Elements = preclassifyproduct.Elements,
                       ClassifyStatus = (Enums.ClassifyStatus)preclassifyproduct.ClassifyStatus,
                       Status = (Enums.Status)preclassifyproduct.Status,
                       CreateDate = preclassifyproduct.CreateDate,
                       UpdateDate = preclassifyproduct.UpdateDate,
                       ClassifyFirstOperator = preclassifyproduct.ClassifyFirstOperator,
                       ClassifySecondOperator = preclassifyproduct.ClassifySecondOperator,
                       Summary = preclassifyproduct.Summary
                   };
        }
    }
}
