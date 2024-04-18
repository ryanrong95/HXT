using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    public class MyMainOrdersView : View<Needs.Wl.Models.MainOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyMainOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal MyMainOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Wl.Models.MainOrder> GetIQueryable()
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

        private IQueryable<Needs.Wl.Models.MainOrder> UserOrders()
        {
            var result = (from mainOrder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()                        
                         join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on mainOrder.UserID equals userTable.ID into users
                         from user in users.DefaultIfEmpty()
                         join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on mainOrder.AdminID equals adminTable.ID into admins
                         from admin in admins.DefaultIfEmpty()
                         where mainOrder.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                         && mainOrder.UserID == this.User.ID
                         select new Needs.Wl.Models.MainOrder
                         {
                             ID = mainOrder.ID,
                             CreateDate = mainOrder.CreateDate,
                         }).Distinct();

            return result;
        }

        private IQueryable<Needs.Wl.Models.MainOrder> ClientOrders()
        {
            var result = (from mainOrder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()                       
                         join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on mainOrder.UserID equals userTable.ID into users
                         from user in users.DefaultIfEmpty()
                         join adminTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on mainOrder.AdminID equals adminTable.ID into admins
                         from admin in admins.DefaultIfEmpty()
                         where mainOrder.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                         && mainOrder.ClientID == this.User.Client.ID
                         select new Needs.Wl.Models.MainOrder
                         {
                             ID = mainOrder.ID,
                             CreateDate = mainOrder.CreateDate,
                         }).Distinct();

            return result;         
        }
    }
}