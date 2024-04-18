using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 我的草稿订单
    /// </summary>
    public class MyDraftOrdersView : View<Needs.Wl.Client.Services.PageModels.DraftOrderViewModel, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyDraftOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.DraftOrderViewModel> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.DraftOrderViewModel> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignees in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal) on order.ID equals consignees.OrderID
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on consignees.ClientSupplierID equals supplier.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Draft
                    && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.DraftOrderViewModel
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       Supplier = new Wl.Models.ClientSupplier()
                       {
                           ID = supplier.ID,
                           Name = supplier.Name,
                           ChineseName = supplier.ChineseName
                       },
                       AgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       Summary = order.Summary
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.DraftOrderViewModel> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignees in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal) on order.ID equals consignees.OrderID
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on consignees.ClientSupplierID equals supplier.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Draft
                    && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.DraftOrderViewModel
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       Supplier = new Wl.Models.ClientSupplier()
                       {
                           ID = supplier.ID,
                           Name = supplier.Name,
                           ChineseName = supplier.ChineseName
                       },
                       AgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       DeclarePrice = order.DeclarePrice,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       Summary = order.Summary
                   };
        }
    }
}