using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyUnPayExchangeOrdersView : View<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyUnPayExchangeOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   //join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(s => s.CusDecStatus != "04") on order.ID equals dechead.OrderID into decheads
                   //from dechead in decheads.DefaultIfEmpty()
                   join paySupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>() on order.ID equals paySupplier.OrderID into suppliers
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal && order.PaidExchangeAmount < order.DeclarePrice
                    && order.OrderStatus >= (int)Needs.Wl.Models.Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= (int)Needs.Wl.Models.Enums.OrderStatus.Completed
                    && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       IsPrePayExchange = clientAgreement.IsPrePayExchange,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       DeclarePrice = items.Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(s => s.TotalPrice),
                       PayExchangeSuppliers = suppliers.Select(item => new Wl.Models.OrderPayExchangeSupplier()
                       {
                           ID = item.ID,
                           ClientSupplierID = item.SupplierID,
                           Name = item.Name,
                           ChineseName = item.ChineseName
                       }),
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       //DeclareDate = dechead == null ? null : dechead.DDate,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   //join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(s => s.CusDecStatus != "04") on order.ID equals dechead.OrderID into decheads
                   //from dechead in decheads.DefaultIfEmpty()
                   join paySupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>() on order.ID equals paySupplier.OrderID into suppliers
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal && order.PaidExchangeAmount < order.DeclarePrice
                    && order.OrderStatus >= (int)Needs.Wl.Models.Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= (int)Needs.Wl.Models.Enums.OrderStatus.Completed
                    && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnPayExchangeOrder
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       IsPrePayExchange = clientAgreement.IsPrePayExchange,
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       DeclarePrice = items.Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal).Sum(s => s.TotalPrice),
                       PayExchangeSuppliers = suppliers.Select(item => new Wl.Models.OrderPayExchangeSupplier()
                       {
                           ID = item.ID,
                           ClientSupplierID = item.SupplierID,
                           Name = item.Name,
                           ChineseName = item.ChineseName
                       }),
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       //DeclareDate = dechead == null ? null : dechead.DDate,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                   };
        }

        public IList<string> GetPayCurrencies()
        {
            var query = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                        where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal && order.PaidExchangeAmount < order.DeclarePrice
                         && order.OrderStatus >= (int)Needs.Wl.Models.Enums.OrderStatus.QuoteConfirmed && order.OrderStatus <= (int)Needs.Wl.Models.Enums.OrderStatus.Completed
                         && order.ClientID == this.User.ClientID
                        orderby order.CreateDate descending
                        select new
                        {
                            order.Currency,
                        }.Currency;

            return query.Distinct().ToList<string>();
        }
    }
}