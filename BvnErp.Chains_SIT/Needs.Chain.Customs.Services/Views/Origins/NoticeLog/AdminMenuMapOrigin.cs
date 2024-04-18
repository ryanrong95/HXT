using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class AdminMenuMapOrigin : UniqueView<Models.AdminMenuMap, ScCustomsReponsitory>
    {
        internal AdminMenuMapOrigin()
        {
        }

        internal AdminMenuMapOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.AdminMenuMap> GetIQueryable()
        {
            return from adminmap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MapsAdminMenuView>()
                   select new Models.AdminMenuMap
                   {
                       AdminID = adminmap.AdminID,
                       MenuID = adminmap.RoleID
                   };
        }
    }
}
