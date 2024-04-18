using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单项商品归类扩展方法
    /// </summary>
    public static class OrderItemCategoryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderItemCategories ToLinq(this Models.OrderItemCategory entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderItemCategories
            {
                ID = entity.ID,
                OrderItemID = entity.OrderItemID,
                ClassifyFirstOperator = entity.ClassifyFirstOperatorID,
                ClassifySecondOperator = entity.ClassifySecondOperatorID,
                Type = (int)entity.Type,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                HSCode = entity.HSCode,
                Unit1 = entity.Unit1,
                Unit2 = entity.Unit2,
                Name = entity.Name,
                Elements = entity.Elements,
                Qty1 = entity.Qty1,
                Qty2 = entity.Qty2,
                CIQCode = entity.CIQCode,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}
