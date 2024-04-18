using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单扩展方法
    /// </summary>
    public static class PreProductCategoriesExtends
    {
        /// <summary>
        /// 归类结果
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.PreProductCategories ToLinq(this Models.ClassifyResult entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PreProductCategories
            {
                ID = entity.ID,
                PreProductID = entity.PreProduct.ID,
                Model = entity.Model,
                Manufacture = entity.Manufacturer,
                ProductName = entity.ProductName,
                HSCode = entity.HSCode,
                TariffRate = entity.TariffRate,
                AddedValueRate = entity.AddedValueRate,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                ClassifyType = entity.ClassifyType==null?null:(int?)entity.ClassifyType,
                Type = entity.Type==null?null:(int?)entity.Type,
                InspectionFee = entity.InspectionFee,
                Unit1 = entity.Unit1,
                Unit2 = entity.Unit2,
                CIQCode = entity.CIQCode,
                Elements = entity.Elements,
                ClassifyStatus = (int)entity.ClassifyStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                ClassifyFirstOperator = entity.ClassifyFirstOperator==null?null:entity.ClassifyFirstOperator.ID,
                ClassifySecondOperator = entity.ClassifySecondOperator==null?null:entity.ClassifySecondOperator.ID,
                Summary = entity.Summary,
                
            };
        }

        /// <summary>
        /// 归类结果更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults ToDefaultLinq(this Models.ClassifyResult entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults
            {
                ID = entity.ID,              
                Model = entity.Model,
                Manufacture = entity.Manufacturer,
                ProductName = entity.ProductName,
                HSCode = entity.HSCode,
                //TariffRate = entity.TariffRate,
                //AddedValueRate = entity.AddedValueRate,
                TaxCode = entity.TaxCode,   
                TaxName = entity.TaxName,           
                InspectionFee = entity.InspectionFee,
                //Unit1 = entity.Unit1,
                //Unit2 = entity.Unit2,
                CIQCode = entity.CIQCode,
                Elements = entity.Elements,               
                Stauts = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,               
            };
        }
    }
}
