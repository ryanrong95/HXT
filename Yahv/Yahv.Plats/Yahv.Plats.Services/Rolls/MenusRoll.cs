using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly.Erps;

namespace Yahv.Plats.Services.Views.Rolls
{
    /// <summary>
    /// 菜单扩展视图
    /// </summary>
    public class MenusRoll : UniqueView<Models.Rolls.Menu, PvbErmReponsitory>
    {
        /// <summary>
        /// 当前参与用例
        /// </summary>
        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin"></param>
        public MenusRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }

        /// <summary>
        /// 集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.Rolls.Menu> GetIQueryable()
        {
            var menus = from menu in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>()
                        select new Models.Rolls.Menu
                        {
                            ID = menu.ID,
                            FatherID = menu.ID,
                            Icon = menu.IconUrl,
                            Name = menu.Name,
                            OrderIndex = menu.OrderIndex,
                            Url = menu.RightUrl
                        };

            if (admin == null || admin.IsSuper)
            {
                return menus;
            }
            else
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRole>()
                       join menu in menus on map.MenuID equals menu.ID
                       where map.RoleID == this.admin.Role.ID
                       select menu;
            }
        }


        /// <summary>
        /// 全部节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Models.Rolls.Menu> GetAllNodes()
        {
            var arry = this.GetIQueryable().Select(item => item).ToArray();

            var linq = from item1 in arry
                       join item2 in arry on item1.ID equals item2.FatherID into sons
                       select new
                       {
                           father = item1,
                           sons = sons
                       };

            var tree = linq.Select(item =>
            {
                item.father.Sons = item.sons.ToArray();
                return item.father;
            });

            return tree;
        }

        /// <summary>
        /// 根据ID获取节点
        /// </summary>
        /// <returns></returns>
        public Models.Rolls.Menu GetNode(string id)
        {
            return this.GetAllNodes().SingleOrDefault(item => item.ID == id);
        }

        /// <summary>
        /// 获取顶级菜单
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Models.Rolls.Menu> GetTops()
        {
            return this.GetAllNodes().Where(item => item.FatherID == null);
        }
    }
}
