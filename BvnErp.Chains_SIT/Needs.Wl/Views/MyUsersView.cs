using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class MyUsersView : UsersView
    {
        IGenericAdmin Admin;

        public MyUsersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.User> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from client in base.GetIQueryable()
                   where client.AdminID == this.Admin.ID
                   select client;
        }
    }
}