using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class PreProductCategoriesView : View<Models.PreProductCategory, ScCustomsReponsitory>
    {
        public PreProductCategoriesView()
        {

        }

        protected override IQueryable<PreProductCategory> GetIQueryable()
        {
            return from classify in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
                   where classify.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.PreProductCategory
                   {
                       ID = classify.ID,
                       PreProductID = classify.PreProductID,
                       Model = classify.Model,
                       Manufacture = classify.Manufacture,
                       ProductName = classify.ProductName,
                       HSCode = classify.HSCode,
                       TariffRate = classify.TariffRate,
                       AddedValueRate = classify.AddedValueRate,
                       TaxCode = classify.TaxCode,
                       TaxName = classify.TaxName,
                       Type = (Enums.ItemCategoryType)classify.Type,
                       ClassifyStatus = (Enums.ClassifyStatus)classify.ClassifyStatus,
                       InspectionFee = classify.InspectionFee,
                       Unit1 = classify.Unit1,
                       Unit2 = classify.Unit2,
                       CIQCode = classify.CIQCode,
                       Elements = classify.Elements,
                       Summary = classify.Summary,
                       Status = (int)classify.Status,
                       CreateDate = classify.CreateDate,
                       UpdateDate = classify.UpdateDate
                   };
        }
    }
}