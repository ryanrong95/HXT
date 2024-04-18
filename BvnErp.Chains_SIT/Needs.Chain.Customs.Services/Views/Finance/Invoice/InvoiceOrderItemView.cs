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
    /// 开票申请的视图
    /// </summary>
    public class InvoiceOrderItemView : UniqueView<Models.InvoiceOrderItem, ScCustomsReponsitory>
    {
        public InvoiceOrderItemView()
        {
        }

        internal InvoiceOrderItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<InvoiceOrderItem> GetIQueryable()
        {
            var orderView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var orderItemView = new OrderItemsView(this.Reponsitory);

            var result = from orderItem in orderItemView
                         join order in orderView on orderItem.OrderID equals order.ID
                         select new Models.InvoiceOrderItem
                         {
                             ID = orderItem.ID,
                             OrderID = orderItem.OrderID,
                             Model = orderItem.Model,
                             Origin = orderItem.Origin,
                             Quantity = orderItem.Quantity,
                             DeclaredQuantity = orderItem.DeclaredQuantity,
                             Unit = orderItem.Unit,
                             UnitPrice = orderItem.UnitPrice,
                             TotalPrice = orderItem.TotalPrice,
                             GrossWeight = orderItem.GrossWeight,
                             IsSampllingCheck = orderItem.IsSampllingCheck,
                             ClassifyStatus = orderItem.ClassifyStatus,
                             Status = orderItem.Status,
                             CreateDate = orderItem.CreateDate,
                             UpdateDate = orderItem.UpdateDate,
                             Summary = orderItem.Summary,
                             Category = orderItem.Category,
                             //海关汇率
                             CustomsRate = order.CustomsExchangeRate == null ? 0M : order.CustomsExchangeRate.Value,
                             //税率
                             ImportTax = orderItem.ImportTax,
                             ExciseTax = orderItem.ExciseTax,
                             AddedValueTax = orderItem.AddedValueTax,
                             InspectionFee = orderItem.InspectionFee,
                             Name = orderItem.Name
                         };
            return result;
        }
    }
}
