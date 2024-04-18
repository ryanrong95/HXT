using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Views
{
    /// <summary>
    /// 个人菜单地址
    /// </summary>
    public class PersonalMenusView : QueryView<string, PvbErmReponsitory>
    {
        private IErpAdmin _erpAdmin;

        public PersonalMenusView(IErpAdmin erpAdmin)
        {
            _erpAdmin = erpAdmin;
        }

        protected override IQueryable<string> GetIQueryable()
        {
            if (_erpAdmin.Role.Type != RoleType.Compose)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRole>()
                    join menu in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>() on map.MenuID equals menu.ID
                    where map.RoleID == _erpAdmin.Role.ID && menu.RightUrl != null
                    select menu.RightUrl;
            }
            else
            {
                var childRoleIds = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                    .Where(item => item.RoleID == _erpAdmin.Role.ID).Select(item => item.ChildID).ToArray();
                return
                    from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRole>()
                    join menu in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>() on map.MenuID equals menu.ID
                    where childRoleIds.Contains(map.RoleID) && menu.RightUrl != null
                    select menu.RightUrl;
            }
        }
    }
}