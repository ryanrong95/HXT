using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 待封箱订单
    /// </summary>
    public class OrderUnSealedsView : UniqueView<Models.Order, ScCustomsReponsitory>
    {
        public OrderUnSealedsView()
        {
        }

        internal OrderUnSealedsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            //已封箱或已出库
            var packingView = new PackingsView(this.Reponsitory).Where(item => item.PackingStatus == Enums.PackingStatus.Sealed ||
                                                                               item.PackingStatus == Enums.PackingStatus.Exited);

            //客户已确认报价，香港库房未到货或未封箱的订单
            return from order in ordersView
                   where order.OrderStatus == Enums.OrderStatus.QuoteConfirmed
                   && !packingView.Any(t => t.OrderID == order.ID)
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
    /// 待封箱订单
    /// </summary>
    public class OrderUnSealedsView1 : Orders1ViewBase<Order>
    {
        public OrderUnSealedsView1()
        {
        }

        internal OrderUnSealedsView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Order> GetIQueryable(Expression<Func<Order, bool>> expression, params LambdaExpression[] expressions)
        {
            //已封箱或已出库
            var packingView = new PackingsView(this.Reponsitory).Where(item => item.PackingStatus == PackingStatus.Sealed ||
                                                                               item.PackingStatus == PackingStatus.Exited);
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == OrderStatus.QuoteConfirmed && !packingView.Any(t => t.OrderID == item.ID));
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }
}
