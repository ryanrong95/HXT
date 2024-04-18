using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 自动归类历史记录的视图
    /// </summary>
    public class ProductCategoriesDefaultsView : UniqueView<Models.ProductCategoriesDefault, ScCustomsReponsitory>
    {
        public ProductCategoriesDefaultsView()
        {
        }

        internal ProductCategoriesDefaultsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProductCategoriesDefault> GetIQueryable()
        {
            var customsTariffs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>();
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>()
                   join custom in customsTariffs on entity.HSCode equals custom.HSCode
                   select new Models.ProductCategoriesDefault
                   {
                       ID = entity.ID,
                       Model = entity.Model,
                       Manufacturer = entity.Manufacture,
                       ProductName = entity.ProductName,
                       HSCode = entity.HSCode,
                       TariffRate = custom.MFN,
                       AddedValueRate = custom.AddedValue,
                       Type = (Enums.ItemCategoryType)entity.Type,
                       //ClassifyType = (Enums.IcgooClassifyTypeEnums)entity.ClassifyType,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       InspectionFee = entity.InspectionFee,
                       Unit1 = custom.Unit1,
                       Unit2 = custom.Unit2,
                       CIQCode = entity.CIQCode,
                       Elements = entity.Elements,
                       Status = (Enums.Status)entity.Stauts,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
