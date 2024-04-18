using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 未开票的订单
    /// TODO:未完成，建议使用数据库视图
    /// </summary>
    public class MyUnComfirmOrdersView : View<Needs.Wl.Client.Services.PageModels.UnComfirmOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyUnComfirmOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.UnComfirmOrder> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnComfirmOrder> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join orderControl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>() on order.ID equals orderControl.OrderID into orderControls
                   from orderControl in orderControls.DefaultIfEmpty()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   join user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on order.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                     && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Quoted
                     && order.IsHangUp == false
                     && order.UserID == this.User.ID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnComfirmOrder
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
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName,
                           IsMain = user.IsMain
                       },
                       Status = order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Summary = order.Summary
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnComfirmOrder> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on order.ClientAgreementID equals clientAgreement.ID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                        && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Quoted
                        && order.IsHangUp == false
                       && order.ClientID == this.User.ClientID
                   orderby order.CreateDate descending
                   select new Needs.Wl.Client.Services.PageModels.UnComfirmOrder
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
                       Summary = order.Summary
                   };
        }
    }
}