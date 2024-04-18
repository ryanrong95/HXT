using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyDecHeadsView : View<Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyDecHeadsView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel> GetIQueryable()
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

        private IQueryable<Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel> UserOrders()
        {
            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on head.OrderID equals order.ID
                   join declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on head.ID equals declist.DeclarationID into declists
                   where order.UserID == this.User.ID && head.IsSuccess == true
                   orderby head.DDate descending
                   select new Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel
                   {
                       ID = head.ID,
                       OrderID = head.OrderID,
                       EntryId = head.EntryId,
                       ContrNo = head.ContrNo,
                       DDate = head.DDate.GetValueOrDefault(),
                       TotalDeclarePrice = declists.Sum(item => item.DeclTotal),
                       Currency = declists.FirstOrDefault().TradeCurr
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel> ClientOrders()
        {
            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on head.OrderID equals order.ID
                   join declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on head.ID equals declist.DeclarationID into declists
                   where order.ClientID == this.User.ClientID && head.IsSuccess == true
                   orderby head.DDate descending
                   select new Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel
                   {
                       ID = head.ID,
                       OrderID = head.OrderID,
                       EntryId = head.EntryId,
                       ContrNo = head.ContrNo,
                       DDate = head.DDate.GetValueOrDefault(),
                       TotalDeclarePrice = declists.Sum(item => item.DeclTotal),
                       Currency = declists.FirstOrDefault().TradeCurr
                   };
        }
    }
}