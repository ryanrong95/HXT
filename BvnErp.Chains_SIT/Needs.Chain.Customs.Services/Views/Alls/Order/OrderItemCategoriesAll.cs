using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    public class OrderItemCategoriesAll : UniqueView<Models.OrderItemCategory, ScCustomsReponsitory>
    {
        public OrderItemCategoriesAll()
        {
        }

        internal OrderItemCategoriesAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderItemCategory> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var itemCategoriesView = new Origins.OrderItemCategoriesOrigin(this.Reponsitory);
            return from itemCategory in itemCategoriesView
                   join admin1 in adminsView on itemCategory.ClassifyFirstOperatorID equals admin1.ID into Admins1
                   from admin1 in Admins1.DefaultIfEmpty()
                   join admin2 in adminsView on itemCategory.ClassifySecondOperatorID equals admin2.ID into Admins2
                   from admin2 in Admins2.DefaultIfEmpty()
                   select new Models.OrderItemCategory
                   {
                       ID = itemCategory.ID,
                       OrderItemID = itemCategory.OrderItemID,

                       Type = itemCategory.Type,
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
                       ClassifyFirstOperatorID = itemCategory.ClassifyFirstOperatorID,
                       ClassifyFirstOperator = admin1,
                       ClassifySecondOperatorID = itemCategory.ClassifySecondOperatorID,
                       ClassifySecondOperator = admin2,
                       Status = itemCategory.Status,
                       CreateDate = itemCategory.CreateDate,
                       UpdateDate = itemCategory.UpdateDate,
                       Summary = itemCategory.Summary
                   };
        }
    }
}
