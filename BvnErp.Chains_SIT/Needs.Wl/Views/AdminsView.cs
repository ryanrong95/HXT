using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class AdminsView : Linq.UniqueFiter<Models.Admin, AdminsToken>
    {
        public AdminsView(string token) : base(new AdminsToken(token))
        {

        }

        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            return this.View.Select(admin => new Models.Admin
            {
                ID = admin.ID,
                UserName = admin.UserName,
                RealName = admin.RealName
            });
        }
    }
}
