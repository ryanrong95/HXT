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
    public static class PreProductCategoriesDefaultExtends    {
       
        /// <summary>
        /// 归类结果更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults ToLinq(this Models.ProductCategoriesDefault entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults
            {
                ID = entity.ID,              
                Model = entity.Model,
                Manufacture = entity.Manufacturer,
                ProductName = entity.ProductName,
                HSCode = entity.HSCode,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                Type = (int?)entity.Type,              
                InspectionFee = entity.InspectionFee,
                CIQCode = entity.CIQCode,
                Elements = entity.Elements,               
                Stauts = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,     
                Summary = entity.Summary,
            };
        }
    }
}
