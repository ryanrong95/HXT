using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 在全部订单，订单跟踪，查询主订单的“订单跟踪”用
    /// </summary>
    public class MainOrdersView : View<Needs.Wl.Models.Order, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MainOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        internal MainOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Needs.Wl.Models.Order> GetIQueryable()
        {
            return this.MainOrders();
        }

        private IQueryable<Needs.Wl.Models.Order> MainOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()                  
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal                  
                   select new Needs.Wl.Models.Order
                   {
                       ID = order.ID,
                       CreateDate = order.CreateDate,
                   };
        }
    }
}