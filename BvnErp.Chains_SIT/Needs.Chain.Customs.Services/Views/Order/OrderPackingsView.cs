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
    public class OrderPackingsView : UniqueView<Models.OrderPacking, ScCustomsReponsitory>
    {
        public OrderPackingsView()
        {
        }

        internal OrderPackingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderPacking> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var packingView = new PackingsView(this.Reponsitory);


            return from order in ordersView
                  // join packing in packingView on order.ID equals packing.OrderID
                   where order.OrderStatus == Enums.OrderStatus.QuoteConfirmed 
                   //&& packingView.Any(t=>t.OrderID == order.ID && t.PackingStatus != Enums.PackingStatus.Exited)
                   select new Models.OrderPacking
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
                       HasPacking = packingView.Any(pack => pack.OrderID == order.ID && pack.Status == Enums.Status.Normal),
                       PackingStatus = packingView.Any(pack => pack.OrderID == order.ID && pack.PackingStatus == Enums.PackingStatus.UnSealed) ? Enums.PackingStatus.UnSealed : Enums.PackingStatus.Sealed
                   };
        }
    }

    /// <summary>
    /// 封箱订单
    /// </summary>
    public class OrderPackingsView1 : Orders1ViewBase<OrderPacking>
    {
        public OrderPackingsView1()
        {
        }

        internal OrderPackingsView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderPacking> GetIQueryable(Expression<Func<OrderPacking, bool>> expression, params LambdaExpression[] expressions)
        {
            return base.GetIQueryable(expression, expressions).Where(item => item.OrderStatus == Enums.OrderStatus.QuoteConfirmed);
        }


        protected override IEnumerable<OrderPacking> OnReadShips(OrderPacking[] results)
        {
            var orderIds = results.Select(o => o.ID).ToArray();
            var clientsView = new ClientsView(this.Reponsitory).Where(item => results.Select(a => a.ClientID).ToArray().Contains(item.ID)).ToArray();
            var clientAgreementsView = new ClientAgreementsView(this.Reponsitory).Where(item => results.Select(a => a.ClientAgreementID).ToArray().Contains(item.ID)).ToArray();
            var packingView = new PackingsView(this.Reponsitory);

            return from order in results
                   join client in clientsView on order.ClientID equals client.ID
                   join clientAgreement in clientAgreementsView on order.ClientAgreementID equals clientAgreement.ID
                   select new OrderPacking
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       ClientID = order.ClientID,
                       Client = client,
                       ClientAgreementID = order.ClientAgreementID,
                       ClientAgreement = clientAgreement,
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
                       HasPacking = packingView.Any(pack => pack.OrderID == order.ID && pack.Status == Enums.Status.Normal),
                       PackingStatus = packingView.Any(pack => pack.OrderID == order.ID && pack.PackingStatus == Enums.PackingStatus.UnSealed) ? Enums.PackingStatus.UnSealed : Enums.PackingStatus.Sealed
                   };
        }

        virtual protected Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            var items = new OrderItemsView(this.Reponsitory);
            return items.Where(item => item.OrderID == orderid).ToArray();
        }
    }
}
