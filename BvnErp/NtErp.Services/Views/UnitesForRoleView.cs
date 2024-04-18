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
    /// 角色颗粒化视图
    /// </summary>
    public class UnitesForRoleView : UnitesAllsView
    {
        Role role;
        public UnitesForRoleView(Role role)
        {
            this.role = role;
        }

        protected override IQueryable<RoleUnite> GetIQueryable()
        {
            if (this.role.ID == "Role000001")
            {
                this.role.ID = "";
            }

            var linqs = from entity in base.GetIQueryable()
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleUnite>()
                        on entity.ID equals map.RoleUniteID
                        where map.RoleID == this.role.ID
                        select entity;

            return linqs;
        }

        /// <summary>
        /// 为当前角色增加颗粒化
        /// </summary>
        /// <param name="id"></param>
        public void Bind(string id)
        {
            var model = base.GetIQueryable().Single(t => t.ID == id);
            if (model == null)
            {
                throw new Exception("unite does not exist!");
            }
            Bind(model);
        }
        /// <summary>
        /// 为当前角色增加颗粒化
        /// </summary>
        /// <param name="id"></param>
        public void Bind(RoleUnite entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                if (!repository.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleUnite>().Any(item => item.RoleID == this.role.ID && item.RoleUniteID == entity.ID))
                {
                    repository.Insert(new Layer.Data.Sqls.BvnErp.MapsRoleUnite
                    {
                        RoleID = this.role.ID,
                        RoleUniteID = entity.ID
                    });
                }
            }
        }
        /// <summary>
        /// 为当前角色移除颗粒化
        /// </summary>
        /// <param name="id"></param>
        public void UnBind(string id)
        {
            var model = base.GetIQueryable().Single(t => t.ID == id);
            if (model == null)
            {
                throw new Exception("unites does not exist!");
            }
            UnBind(model);
        }
        /// <summary>
        /// 为当前角色移除颗粒化
        /// </summary>
        /// <param name="id"></param>
        public void UnBind(RoleUnite entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnErp.MapsRoleUnite>(item => item.RoleID == this.role.ID && item.RoleUniteID == entity.ID);
            }
        }

    }
}
