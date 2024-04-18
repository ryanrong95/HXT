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
    /// 代理订单项的视图
    /// </summary>
    public class OrderBillItemsView : UniqueView<Models.OrderItem, ScCustomsReponsitory>
    {
        public OrderBillItemsView()
        {
        }

        public OrderBillItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderItem> GetIQueryable()
        {
            var categoriesView = new OrderItemCategoriesView(this.Reponsitory);
            var taxesView = new OrderItemTaxesView(this.Reponsitory);
            var premiumsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();

            return from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()                 
                   join category in categoriesView on orderItem.ID equals category.OrderItemID into categories
                   from category in categories.DefaultIfEmpty()
                   join importTax in taxesView.Where(t => t.Type == Enums.CustomsRateType.ImportTax) on orderItem.ID equals importTax.OrderItemID into importTaxes
                   from importTax in importTaxes.DefaultIfEmpty()
                   join addedValueTax in taxesView.Where(t => t.Type == Enums.CustomsRateType.AddedValueTax) on orderItem.ID equals addedValueTax.OrderItemID into addedValueTaxes
                   from addedValueTax in addedValueTaxes.DefaultIfEmpty()
                   join inspFee in premiumsView.Where(f => f.Type == (int)Enums.OrderPremiumType.InspectionFee) on orderItem.ID equals inspFee.OrderItemID into inspFees
                   from inspFee in inspFees.DefaultIfEmpty()
                   orderby orderItem.ID
                   select new Models.OrderItem
                   {
                       ID = orderItem.ID,
                       OrderID = orderItem.OrderID,
                       Name=orderItem.Name,
                       Manufacturer=orderItem.Manufacturer,
                       Model=orderItem.Model,
                       Batch=orderItem.Batch,
                       Origin = orderItem.Origin,
                       Quantity = orderItem.Quantity,
                       DeclaredQuantity = orderItem.DeclaredQuantity,
                       Unit = orderItem.Unit,
                       UnitPrice = orderItem.UnitPrice,
                       TotalPrice = orderItem.TotalPrice,
                       GrossWeight = orderItem.GrossWeight,
                       IsSampllingCheck = orderItem.IsSampllingCheck,
                       ClassifyStatus = (Enums.ClassifyStatus)orderItem.ClassifyStatus,
                       Status = (Enums.Status)orderItem.Status,
                       CreateDate = orderItem.CreateDate,
                       UpdateDate = orderItem.UpdateDate,
                       Summary = orderItem.Summary,
                       Category = category,
                       ImportTax = importTax,
                       AddedValueTax = addedValueTax,
                       InspectionFee = inspFee == null ? null : (decimal?)inspFee.UnitPrice * inspFee.Count * inspFee.Rate,
                       ProductUniqueCode = orderItem.ProductUniqueCode,
                   };
        }
    }
}
