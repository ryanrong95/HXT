using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品归类历史扩展方法
    /// </summary>
    public static class ProductCategoryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ProductCategories ToLinq(this Models.ProductCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ProductCategories
            {
                ID = entity.ID,
                Model = entity.Model,
                Name = entity.Name,
                HSCode = entity.HSCode,
                TariffRate = entity.TariffRate,
                AddedValueRate = entity.AddedValueRate,
                UnitPrice = entity.UnitPrice,
                InspectionFee = entity.InspectionFee,
                Quantity = entity.Qty,
                Elements = entity.Elements,
                AdminID = entity.Declarant.ID,
                CreateDate = entity.CreateDate,
            };
        }
    }
}
