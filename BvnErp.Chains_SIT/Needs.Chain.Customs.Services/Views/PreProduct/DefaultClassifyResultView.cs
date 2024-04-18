using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DefaultClassifyResultView : UniqueView<Models.ClassifyResult, ScCustomsReponsitory>
    {
        public DefaultClassifyResultView()
        {
        }

        internal DefaultClassifyResultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifyResult> GetIQueryable()
        {
            var customsTariffs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>();
            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>()
                   join custom in customsTariffs on para.HSCode equals custom.HSCode
                   select new Models.ClassifyResult
                   {
                      ID = para.ID,
                      Model = para.Model,
                      Manufacturer = para.Manufacture,
                      ProductName = para.ProductName,
                      HSCode = para.HSCode,
                      TariffRate = custom.MFN,
                      AddedValueRate = custom.AddedValue,
                      TaxCode = para.TaxCode,   
                      TaxName = para.TaxName,
                      InspectionFee = para.InspectionFee,
                      Unit1 = custom.Unit1,
                      Unit2 = custom.Unit2,
                      CIQCode = para.CIQCode,
                      Elements = para.Elements,
                      Status = (Status)para.Stauts,
                      CreateDate = para.CreateDate,
                      UpdateDate = para.UpdateDate,
                      //ClassifyType = (IcgooClassifyTypeEnums)para.ClassifyType,
                      Type = (ItemCategoryType)para.Type,
                   };
        }
    }
}
