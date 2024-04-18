using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Web.Views
{
    class YahvAdminsView : QueryView<Models.Admin, PvbErmReponsitory>
    {
        internal YahvAdminsView()
        {
        }

        protected override IQueryable<Models.Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbErm.Admins>()
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                   };
        }
    }
}
