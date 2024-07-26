using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class NoticeBoardAdminRolesView : QueryView<NoticeBoardModel, PvbErmReponsitory>
    {
        public NoticeBoardAdminRolesView()
        {
        }

        protected NoticeBoardAdminRolesView(PvbErmReponsitory pvbErmReponsitory, IQueryable<NoticeBoardModel> iQueryable) : base(pvbErmReponsitory, iQueryable)
        {
        }

        protected override IQueryable<NoticeBoardModel> GetIQueryable()
        {
            return from mapsRole in this.Reponsitory.ReadTable<Layer.Data.Sqls.PvbErm.Roles>()
                   where mapsRole.Name.Contains("报关") || mapsRole.Name.Contains("华芯通")
                   && mapsRole.Status != (int)Enums.Status.Delete
                   select new NoticeBoardModel
                   {
                       RoleID = mapsRole.ID,
                       RoleName = mapsRole.Name
                   };
        }
    }
}
