using Needs.Erp.Generic;
using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 管理员菜单视图
    /// </summary>
    public class MyMenusView : MenusAlls
    {
        IGenericAdmin admin;
        public MyMenusView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<Menu> GetIQueryable()
        {
            if (admin.IsSa)
            {
                return base.GetIQueryable();
            }
            else
            {
                var roles = (from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminRole>()
                             where entity.AdminID == admin.ID
                             select entity.RoleID).ToArray();

                var linqs = from entity in base.GetIQueryable()
                            join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleMenu>()
                            on entity.ID equals map.MenuID
                            where roles.Contains(map.RoleID)
                            select entity;

                return linqs;
            }
        }


    }
}
