using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class OrderItemCategoriesOrigin : UniqueView<Models.OrderItemCategory, ScCustomsReponsitory>
    {
        internal OrderItemCategoriesOrigin()
        {
        }

        internal OrderItemCategoriesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderItemCategory> GetIQueryable()
        {
            return from itemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                   select new Models.OrderItemCategory
                   {
                       ID = itemCategory.ID,
                       OrderItemID = itemCategory.OrderItemID,
                      
                       Type = (Enums.ItemCategoryType)itemCategory.Type,
                       TaxCode = itemCategory.TaxCode,
                       TaxName = itemCategory.TaxName,
                       HSCode = itemCategory.HSCode,
                       Name = itemCategory.Name,
                       Elements = itemCategory.Elements,
                       Unit1 = itemCategory.Unit1,
                       Unit2 = itemCategory.Unit2,
                       Qty1 = itemCategory.Qty1,
                       Qty2 = itemCategory.Qty2,
                       CIQCode = itemCategory.CIQCode,
                       ClassifyFirstOperatorID = itemCategory.ClassifyFirstOperator,
                       ClassifySecondOperatorID = itemCategory.ClassifySecondOperator,
                       Status = (Enums.Status)itemCategory.Status,
                       CreateDate = itemCategory.CreateDate,
                       UpdateDate = itemCategory.UpdateDate,
                       Summary = itemCategory.Summary
                   };
        }
    }
}
