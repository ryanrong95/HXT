using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyRejectedOrdersView : View<Needs.Wl.Client.Services.PageModels.RejectedOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyRejectedOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.RejectedOrder> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.RejectedOrder> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignees in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal) on order.ID equals consignees.OrderID
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on consignees.ClientSupplierID equals supplier.ID
                   //join log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>().OrderByDescending(s => s.CreateDate).Where(s => s.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned) on order.ID equals log.OrderID into logs
                   //from log in logs.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned
                    && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.RejectedOrder
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
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       //ReturnedSummary = log.Summary
                       ReturnedSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.RejectedOrder> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignees in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Where(s => s.Status == (int)Needs.Wl.Models.Enums.Status.Normal) on order.ID equals consignees.OrderID
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on consignees.ClientSupplierID equals supplier.ID
                   //join log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>().OrderByDescending(s => s.CreateDate).Where(s => s.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned) on order.ID equals log.OrderID into logs
                   //from log in logs.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned
                    && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.RejectedOrder
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
                       InvoiceStatus = (Needs.Wl.Models.Enums.InvoiceStatus)order.InvoiceStatus,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       IsHangUp = order.IsHangUp,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary,
                       //ReturnedSummary = log.Summary
                       ReturnedSummary = (from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderLogs>()
                                          where log.OrderID == order.ID && log.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Returned
                                          orderby log.CreateDate descending
                                          select log.Summary).FirstOrDefault()
                   };
        }
    }
}