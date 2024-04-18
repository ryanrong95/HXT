using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Web.Models;
using System.Linq;

namespace Needs.Web.Views
{
    class AdminsView : QueryView<Admin, BvnErpReponsitory>
    {
        internal AdminsView()
        {

        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            return from admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvTester.Admins>()
                   select new Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                   };
        }
    }
}
