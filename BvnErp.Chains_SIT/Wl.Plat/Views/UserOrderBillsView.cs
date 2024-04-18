using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.User.Plat.Models;

namespace Needs.Wl.User.Plat.Views
{
   public class UserOrderBillsView: OrderBillsView
    {
        IPlatUser User;

        internal UserOrderBillsView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderBill> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return from entity in base.GetIQueryable()
                       where entity.Client.ID== this.User.Client.ID
                       select entity;
            }
            else
            {
                return from entity in base.GetIQueryable()
                       where entity.Order.UserID == this.User.ID 
                       select entity;
            }
        }
    }
}
