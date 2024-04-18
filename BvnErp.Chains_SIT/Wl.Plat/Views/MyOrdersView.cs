using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyOrdersView : View<Needs.Wl.Models.Order, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal MyOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Wl.Models.Order> GetIQueryable()
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

        private IQueryable<Needs.Wl.Models.Order> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on order.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on order.AdminID equals adminTable.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   && order.UserID == this.User.ID
                   select new Needs.Wl.Models.Order
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Admin = admin == null ? null : new Needs.Wl.Models.Admin()
                       {
                           ID = admin.ID,
                           UserName = admin.UserName,
                           RealName = admin.RealName
                       },
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
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

        private IQueryable<Needs.Wl.Models.Order> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on order.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on order.AdminID equals adminTable.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   && order.ClientID == this.User.Client.ID
                   select new Needs.Wl.Models.Order
                   {
                       ID = order.ID,
                       Type = (Needs.Wl.Models.Enums.OrderType)order.Type,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Admin = admin == null ? null : new Needs.Wl.Models.Admin()
                       {
                           ID = admin.ID,
                           UserName = admin.UserName,
                           RealName = admin.RealName
                       },
                       Currency = order.Currency,
                       CustomsExchangeRate = order.CustomsExchangeRate,
                       RealExchangeRate = order.RealExchangeRate,
                       IsFullVehicle = order.IsFullVehicle,
                       IsLoan = order.IsLoan,
                       PackNo = order.PackNo,
                       WarpType = order.WarpType,
                       DeclarePrice = order.DeclarePrice,
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