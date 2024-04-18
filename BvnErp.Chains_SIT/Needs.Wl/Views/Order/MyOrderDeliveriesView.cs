using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class MyOrderDeliveriesView : OrderDeliveriesView
    {
        IGenericAdmin Admin;

        public MyOrderDeliveriesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }
}
