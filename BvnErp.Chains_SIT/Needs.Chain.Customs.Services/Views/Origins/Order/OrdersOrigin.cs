using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class OrdersOrigin : UniqueView<Models.Order, ScCustomsReponsitory>
    {
        public OrdersOrigin()
        {
        }

        public OrdersOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   select new Models.Order
                   {
                       ID = order.ID,
                       MainOrderID = order.MainOrderId,
                       Type = (Enums.OrderType)order.Type,
                       AdminID = order.AdminID,
                       UserID = order.UserID,
                       ClientID = order.ClientID,
                       ClientAgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                       Status = (Enums.Status)order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                   };
        }
    }
}
