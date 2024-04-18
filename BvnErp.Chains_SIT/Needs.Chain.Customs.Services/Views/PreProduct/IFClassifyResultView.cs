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
    public class IFClassifyResultView : UniqueView<Models.ClassifyResult, ScCustomsReponsitory>
    {
        public IFClassifyResultView()
        {
        }

        internal IFClassifyResultView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifyResult> GetIQueryable()
        {

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClassifyResultView>()                  
                   select new Models.ClassifyResult
                   {
                       ID = para.ID,
                       PreProductID = para.PreProductID,
                       Model = para.Model,
                       Manufacturer = para.Manufacturer,
                       ProductName = para.ProductName,
                       HSCode = para.HSCode,
                       TariffRate = para.TariffRate,
                       AddedValueRate = para.AddedValueRate,
                       ExciseTaxRate = para.ExciseTaxRate,
                       TaxCode = para.TaxCode,
                       TaxName = para.TaxName,
                       ClassifyType = (IcgooClassifyTypeEnums)para.ClassifyType,
                       Type = (ItemCategoryType)para.Type,
                       ClassifyStatus = (ClassifyStatus)para.ClassifyStatus,
                       InspectionFee = para.InspectionFee,
                       Unit1 = para.Unit1,
                       Unit2 = para.Unit2,
                       CIQCode = para.CIQCode,
                       Elements = para.Elements,
                       Status = (Status)para.Status,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       DeclarantID = para.DeclarantID,
                       ClassifyFirstOperatorID = para.ClassifyFirstOperatorID,
                       ClassifySecondOperatorID = para.ClassifySecondOperatorID,
                       PreProductUnicode = para.PreProductUnicode,
                       CompanyType = (CompanyTypeEnums)para.CompanyType,
                   };
        }

        //protected override IQueryable<Models.ClassifyResult> GetIQueryable()
        //{

        //    return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
        //           join pre in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>() on para.PreProductID equals pre.ID
        //           select new Models.ClassifyResult
        //           {
        //               ID = para.ID,
        //               PreProductID = para.PreProductID,
        //               Model = para.Model,
        //               Manufacturer = para.Manufacture,
        //               ProductName = para.ProductName,
        //               HSCode = para.HSCode,
        //               TariffRate = para.TariffRate,
        //               AddedValueRate = para.AddedValueRate,
        //               ExciseTaxRate = para.ExciseTaxRate,
        //               TaxCode = para.TaxCode,
        //               TaxName = para.TaxName,
        //               ClassifyType = (IcgooClassifyTypeEnums)para.ClassifyType,
        //               Type = (ItemCategoryType)para.Type,
        //               ClassifyStatus = (ClassifyStatus)para.ClassifyStatus,
        //               InspectionFee = para.InspectionFee,
        //               Unit1 = para.Unit1,
        //               Unit2 = para.Unit2,
        //               CIQCode = para.CIQCode,
        //               Elements = para.Elements,
        //               Status = (Status)para.Status,
        //               CreateDate = para.CreateDate,
        //               UpdateDate = para.UpdateDate,
        //               DeclarantID = para.ClassifySecondOperator,
        //               ClassifyFirstOperatorID = para.ClassifyFirstOperator,
        //               ClassifySecondOperatorID = para.ClassifySecondOperator,
        //               PreProductUnicode = pre.ProductUnionCode,
        //               CompanyType = (CompanyTypeEnums)pre.CompanyType,
        //           };
        //}
    }
}
