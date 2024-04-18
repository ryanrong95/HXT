using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Views
{
    public class YahvAdminAlls : UniqueView<Needs.Wl.Admin.Plat.Models.Admin, PvbErmReponsitory>
    {
        protected internal YahvAdminAlls()
        {
        }

        protected override IQueryable<Needs.Wl.Admin.Plat.Models.Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbErm.Admins>()
                   select new Needs.Wl.Admin.Plat.Models.Admin
                   {
                       ID = admin.ID,
                       RealName = admin.RealName,
                       UserName = admin.UserName,
                       OriginID = admin.OriginID
                   };
        }
    }
}
