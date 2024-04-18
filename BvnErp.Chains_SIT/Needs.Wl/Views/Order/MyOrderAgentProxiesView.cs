using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class MyOrderAgentProxiesView : OrderAgentProxiesView
    {
        IGenericAdmin Admin;

        public MyOrderAgentProxiesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderAgentProxy> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from agentProxy in base.GetIQueryable()
                   where agentProxy.Client.Merchandiser.ID == this.Admin.ID
                   select agentProxy;
        }
    }
}
