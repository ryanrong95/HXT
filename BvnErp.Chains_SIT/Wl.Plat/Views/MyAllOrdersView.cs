using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 我的全部订单
    /// </summary>
    public class MyAllOrdersView : View<Needs.Wl.Client.Services.PageModels.AllOrderViewModel, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyAllOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal MyAllOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.AllOrderViewModel> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.AllOrderViewModel> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join orderType in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>() on order.ID equals orderType.OrderID into orderTypes
                   join isUnAuditedControl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IsUnAuditedControlView>() on order.ID equals isUnAuditedControl.OrderID into isUnAuditedControlViews
                   from isUnAuditedControl in isUnAuditedControlViews.DefaultIfEmpty()
                   join paySupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>() on order.ID equals paySupplier.OrderID into paySuppliers
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.AllOrderViewModel
                   {
                       ID = order.ID,
                       OrderVoyages = orderTypes.Select(item => new Needs.Wl.Models.OrderVoyage
                       {
                           Type = (Needs.Wl.Models.Enums.OrderSpecialType)item.Type
                       }),
                       IsPrePayExchange = clientAgreement.IsPrePayExchange,
                       Currency = order.Currency,
                       PayExchangeSuppliers = paySuppliers.Select(item => new Wl.Models.OrderPayExchangeSupplier()
                       {
                           ID = item.ID,
                           ClientSupplierID = item.SupplierID,
                           Name = item.Name,
                           ChineseName = item.ChineseName
                       }),
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       IsBecauseModified = isUnAuditedControl != null ?
                                            (order.IsHangUp == true) && (isUnAuditedControl.IsUnAuditedDeleteModel > 0 || isUnAuditedControl.IsUnAuditedChangeQuantity > 0)
                                            : false,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.AllOrderViewModel> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join orderType in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderVoyages>() on order.ID equals orderType.OrderID into orderTypes
                   join isUnAuditedControl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IsUnAuditedControlView>() on order.ID equals isUnAuditedControl.OrderID into isUnAuditedControlViews
                   from isUnAuditedControl in isUnAuditedControlViews.DefaultIfEmpty()
                   join paySupplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>() on order.ID equals paySupplier.OrderID into paySuppliers
                   join mainorder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>() on order.MainOrderId equals mainorder.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                        && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.AllOrderViewModel
                   {
                       ID = order.ID,
                       OrderVoyages = orderTypes.Select(item => new Needs.Wl.Models.OrderVoyage
                       {
                           Type = (Needs.Wl.Models.Enums.OrderSpecialType)item.Type
                       }),
                       IsPrePayExchange = clientAgreement.IsPrePayExchange,
                       Currency = order.Currency,
                       PayExchangeSuppliers = paySuppliers.Select(item => new Wl.Models.OrderPayExchangeSupplier()
                       {
                           ID = item.ID,
                           ClientSupplierID = item.SupplierID,
                           Name = item.Name,
                           ChineseName = item.ChineseName
                       }),
                       DeclarePrice = order.DeclarePrice,
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       IsBecauseModified = isUnAuditedControl != null ?
                                            (order.IsHangUp == true) && (isUnAuditedControl.IsUnAuditedDeleteModel > 0 || isUnAuditedControl.IsUnAuditedChangeQuantity > 0)
                                            : false,
                       MainOrderID = order.MainOrderId,
                       MainOrderCreateDate = mainorder.CreateDate,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                   };
        }
    }
}