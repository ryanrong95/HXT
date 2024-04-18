using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 颗粒化视图
    /// </summary>
    public class MapsAdminRoleView : QueryView<MapAdminRole, BvnErpReponsitory>
    {
        IGenericAdmin admin;
        Role role;
        public MapsAdminRoleView()
        {

        }

        public MapsAdminRoleView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        public MapsAdminRoleView(Role role)
        {
            this.role = role;
        }

        protected override IQueryable<MapAdminRole> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminRole>()
                        select new MapAdminRole
                        {
                            AdminID = entity.AdminID,
                            RoleID = entity.RoleID
                        };
            if (this.admin != null)
            {
                linqs = linqs.Where(item => item.AdminID == this.admin.ID);
            }
            if (this.role != null)
            {
                linqs = linqs.Where(item => item.RoleID == this.role.ID);
            }
            return linqs;
        }
    }
}
