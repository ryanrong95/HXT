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
    /// 管理员颗粒化视图
    /// </summary>
    public class MyUnitesView : UnitesAllsView
    {
        IGenericAdmin admin;
        public MyUnitesView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<RoleUnite> GetIQueryable()
        {
            

            var roles = (from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminRole>()
                         where entity.AdminID == admin.ID
                         select entity.RoleID).ToArray();

            if (this.admin.IsSa)
            {
                roles = new string[0];
            }

            var linqs = from entity in base.GetIQueryable()
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleUnite>() 
                        on entity.ID equals map.RoleUniteID
                        where roles.Contains(map.RoleID)
                        select new RoleUnite
                        {
                            ID = entity.ID,
                            Type = (RoleUniteType)entity.Type,
                            Menu = entity.Menu,
                            Name = entity.Name,
                            Title = entity.Title,
                            Url = entity.Url,
                            CreateDate = entity.CreateDate
                        };

            return linqs;
        }
    }
}
