using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class PvbErmOrigin : UniqueView<Models.PvbErmAdminMap, ScCustomsReponsitory>
    {
        public PvbErmOrigin()
        {
        }

        internal PvbErmOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PvbErmAdminMap> GetIQueryable()
        {
            return from adminmap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvbErmAdmins>()
                   select new Models.PvbErmAdminMap
                   {
                       ID = adminmap.ID,
                       OriginID = adminmap.OriginID,
                       RealName = adminmap.RealName,
                       UserName = adminmap.UserName,
                       RoleID = adminmap.RoleID
                   };
        }
    }
}
