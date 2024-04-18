using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderArrivalsView : UniqueView<Models.Order, ScCustomsReponsitory>
    {
        public OrderArrivalsView()
        {
        }

        internal OrderArrivalsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var packingView = new PackingsView(this.Reponsitory);


            return from order in ordersView
                   //join packing in packingView on order.ID equals packing.OrderID
                   where order.OrderStatus == Enums.OrderStatus.QuoteConfirmed
                   && packingView.Any(t=>t.OrderID == order.ID && t.PackingStatus != Enums.PackingStatus.Exited)
                   select new Models.Order
                   {
                       ID = order.ID,
                       Client = order.Client,
                       ClientAgreement = order.ClientAgreement,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                   };
        }
    }

    /// <summary>
    ///已取消订单的视图
    /// </summary>
    public class OrderArrivalsView1 : Orders1ViewBase<Models.Order>
    {
        public OrderArrivalsView1() : base()
        {
        }

        internal OrderArrivalsView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable(Expression<Func<Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            var packingView = new PackingsView(this.Reponsitory);

            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.QuoteConfirmed
                   && packingView.Any(t => t.OrderID == item.ID && t.PackingStatus != Enums.PackingStatus.Exited));
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }
}
