using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 未开票的订单
    /// </summary>
    public class MyUnInvocieOrdersView : View<Needs.Wl.Client.Services.PageModels.UnInvocieOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyUnInvocieOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.UnInvocieOrder> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return this.ClientOrders();
            }
            else
            {
                return this.UserOrders();
            }
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnInvocieOrder> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.InvoiceStatus == (int)Needs.Wl.Models.Enums.InvoiceStatus.UnInvoiced
                    && order.OrderStatus >= (int)Needs.Wl.Models.Enums.OrderStatus.Declared
                    && order.OrderStatus <= (int)Needs.Wl.Models.Enums.OrderStatus.Completed
                    && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnInvocieOrder
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       InvoiceType = (Needs.Wl.Models.Enums.InvoiceType)clientAgreement.InvoiceType,
                       InvoiceTaxRate = clientAgreement.InvoiceTaxRate,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       DeclarePrice = items.Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(s => s.TotalPrice),
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnInvocieOrder> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.InvoiceStatus == (int)Needs.Wl.Models.Enums.InvoiceStatus.UnInvoiced
                    && order.OrderStatus >= (int)Needs.Wl.Models.Enums.OrderStatus.Declared
                    && order.OrderStatus <= (int)Needs.Wl.Models.Enums.OrderStatus.Completed
                    && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnInvocieOrder
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       InvoiceType = (Needs.Wl.Models.Enums.InvoiceType)clientAgreement.InvoiceType,
                       InvoiceTaxRate = clientAgreement.InvoiceTaxRate,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       DeclarePrice = items.Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(s => s.TotalPrice),
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                   };
        }
    }
}