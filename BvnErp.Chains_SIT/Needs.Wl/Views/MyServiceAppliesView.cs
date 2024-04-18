using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public sealed class MyServiceAppliesView : ServiceAppliesView
    {
        IGenericAdmin Admin;

        public MyServiceAppliesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ServiceApplies> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from apply in base.GetIQueryable()
                   where apply.Admin.ID == this.Admin.ID
                   || apply.Admin == null
                   select apply;
        }
    }
}
