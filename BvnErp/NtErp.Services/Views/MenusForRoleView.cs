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
    /// 角色菜单视图
    /// </summary>
    public class MenusForRoleView : MenusAlls
    {
        Role role;
        public MenusForRoleView(Role role)
        {
            this.role = role;
        }

        protected override IQueryable<Menu> GetIQueryable()
        {
            if (this.role.ID == "Role000001")
            {
                return base.GetIQueryable();
            }
            var linqs = from entity in base.GetIQueryable()
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleMenu>()
                        on entity.ID equals map.MenuID
                        where map.RoleID == this.role.ID
                        select entity;

            return linqs;

        }

        /// <summary>
        /// 为当前角色增加菜单
        /// </summary>
        /// <param name="id"></param>
        public void Bind(string id)
        {
            var menu = base.GetIQueryable().Single(t => t.ID == id);
            if (menu == null)
            {
                throw new Exception("menu does not exist!");
            }
            Bind(menu);
        }
        /// <summary>
        /// 为当前角色增加菜单
        /// </summary>
        /// <param name="id"></param>
        public void Bind(Menu entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                if (!repository.ReadTable<Layer.Data.Sqls.BvnErp.MapsRoleMenu>().Any(item => item.RoleID == this.role.ID && item.MenuID == entity.ID))
                {
                    repository.Insert(new Layer.Data.Sqls.BvnErp.MapsRoleMenu
                    {
                        RoleID = this.role.ID,
                        MenuID = entity.ID
                    });
                }
            }
        }
        /// <summary>
        /// 为当前角色移除菜单
        /// </summary>
        /// <param name="id"></param>
        public void UnBind(string id)
        {
            var menu = base.GetIQueryable().Single(t => t.ID == id);
            if (menu == null)
            {
                throw new Exception("menu does not exist!");
            }
            UnBind(menu);
        }
        /// <summary>
        /// 为当前角色移除菜单
        /// </summary>
        /// <param name="id"></param>
        public void UnBind(Menu entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnErp.MapsRoleMenu>(item => item.RoleID == this.role.ID && item.MenuID == entity.ID);
            }
        }

    }
}
