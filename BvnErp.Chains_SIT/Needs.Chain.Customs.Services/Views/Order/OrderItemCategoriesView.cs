using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单项商品归类的视图
    /// </summary>
    public class OrderItemCategoriesView : UniqueView<Models.OrderItemCategory, ScCustomsReponsitory>
    {
        public OrderItemCategoriesView()
        {
        }

        internal OrderItemCategoriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItemCategory> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from itemCategory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                   join admin1 in adminsView on itemCategory.ClassifyFirstOperator equals admin1.ID into Admin1
                   from admin1 in Admin1.DefaultIfEmpty()
                   join admin2 in adminsView on itemCategory.ClassifySecondOperator equals admin2.ID into Admin2
                   from admin2 in Admin2.DefaultIfEmpty()
                   select new Models.OrderItemCategory
                   {
                       ID = itemCategory.ID,
                       OrderItemID = itemCategory.OrderItemID,
                       ClassifyFirstOperator = admin1,
                       ClassifySecondOperator = admin2,
                       Type = (Enums.ItemCategoryType)itemCategory.Type,
                       TaxCode = itemCategory.TaxCode,
                       TaxName = itemCategory.TaxName,
                       HSCode = itemCategory.HSCode,
                       Unit1 = itemCategory.Unit1,
                       Unit2 = itemCategory.Unit2,
                       Name = itemCategory.Name,
                       Elements = itemCategory.Elements,
                       Qty1 = itemCategory.Qty1,
                       Qty2 = itemCategory.Qty2,
                       CIQCode = itemCategory.CIQCode,
                       Status = (Enums.Status)itemCategory.Status,
                       CreateDate = itemCategory.CreateDate,
                       UpdateDate = itemCategory.UpdateDate,
                       Summary = itemCategory.Summary
                   };
        }
    }
}
