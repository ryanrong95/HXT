using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class OrdersRoll : UniqueView<Models.Order, ScCustomsReponsitory>
    {
        internal OrdersRoll()
        {
        }

        internal OrdersRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        } 

        protected override IQueryable<Order> GetIQueryable()
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory);
            var clients = new Rolls.ClientsRoll(this.Reponsitory);
            var clientAgreements = new Rolls.ClientAgreementsRoll(this.Reponsitory);
            var orderConsignees = new Rolls.OrderConsigneesRoll(this.Reponsitory);

            return from order in orders
                   join client in clients on order.ClientID equals client.ID
                   join clientAgreement in clientAgreements on order.ClientAgreementID equals clientAgreement.ID
                   join orderConsignee in orderConsignees on order.ID equals orderConsignee.OrderID
                   select new Models.Order
                   {
                       ID = order.ID,
                       Type = order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       Client = client,
                       ClientAgreement = clientAgreement,
                       OrderConsignee = orderConsignee,
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
}
