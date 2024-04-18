using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class OrdersView : View<Needs.Wl.Models.Order, ScCustomsReponsitory>
    {
        public OrdersView()
        {

        }

        internal OrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Wl.Models.Order> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals item.OrderID into items
                   join user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on order.UserID equals user.ID into users
                   from user in users.DefaultIfEmpty()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on order.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
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
    }
}
