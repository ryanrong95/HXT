using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Linq;
using Yahv.Utils.Serializers;
using Yahv.Plats.Services.Models.Origins;
using Yahv.Underly.Erps;
using System.Linq.Expressions;

namespace Yahv.Plats.Services.Views
{
    /// <summary>
    /// 菜单视图 （重新命名）
    /// </summary>
    public class MyMenus : UniqueView<Menu, PvbErmReponsitory>
    {
        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyMenus(IErpAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<Menu> GetIQueryable()
        {
            //根据admin 的 mapsrole 获取到他可以看到的 数据的行

            var linq = from entity in this.Reponsitory.ReadTable<Menus>()
                       where entity.IsLocal != false       //只显示本地
                       select new Menu()
                       {
                           ID = entity.ID,
                           Status = (Status)entity.Status,
                           Name = entity.Name,
                           Company = entity.Company,
                           OrderIndex = entity.OrderIndex,
                           IconUrl = entity.IconUrl,
                           FirstUrl = entity.FirstUrl,
                           RightUrl = entity.RightUrl,
                           FatherID = entity.FatherID,
                           LogoUrl = entity.LogoUrl,
                           HelpUrl = entity.HelpUrl,
                       };

            if (admin.IsSuper)
            {
                return linq;
            }

            Expression<Func<MapsRole, bool>> expression;

            //根据类型获取 条件
            if (admin.Role.Type == Underly.RoleType.Compose)
            {
                var roleIds_linq = from map in this.Reponsitory.ReadTable<MapsRoleCompose>()
                                   where map.RoleID == this.admin.Role.ID
                                   select map.ChildID;

                var roleIds = roleIds_linq.Distinct().ToArray();

                //菜单用 并集，只要有就算上
                expression = item => roleIds.Contains(item.RoleID);
                //这里需要我们的商议目前的公式：[补(A交B)]并(A交B)，这个公式同时运用到权限菜单+颗粒化中
            }
            else
            {
                expression = item => item.RoleID == admin.Role.ID;
            }

            var maps = from map in this.Reponsitory.ReadTable<MapsRole>().Where(expression)
                       select map.MenuID;

            return from entity in linq
                   join map in maps.Distinct() on entity.ID equals map
                   select entity;
        }

        public override string ToString()
        {
            return this.Json();
        }

        public string Json()
        {
            var arry = this.Where(item => item.Status == Status.Normal).ToArray();

            var linq = from business in arry
                       where business.FatherID == null
                       orderby business.OrderIndex
                       select new
                       {
                           ID = business.ID,
                           Name = business.Name,
                           IconUrl = business.IconUrl,
                           LogoUrl = business.LogoUrl,
                           FirstUrl = business.FirstUrl,
                           HelpUrl = business.HelpUrl,
                           Company = business.Company,
                           Menu = (from first in arry
                                   where first.FatherID == business.ID
                                   select new
                                   {
                                       text = first.Name,
                                       orderIndex = first.OrderIndex,
                                       children = (from second in arry
                                                   where second.FatherID == first.ID

                                                   select new
                                                   {
                                                       text = second.Name,
                                                       orderIndex = second.OrderIndex,
                                                       url = second.RightUrl
                                                   }).OrderBy(item => item.orderIndex).ToArray()
                                   }).OrderBy(item => item.orderIndex).ToArray()
                       };

            return linq.ToArray().Json();
        }

        /// <summary>
        /// 依据荣检要求,返回芯达通
        /// </summary>
        public bool IsXdt
        {
            get
            {
                if (this.admin.IsSuper)
                {
                    return false;
                }

                var arry = this.GetIQueryable().ToArray();

                return arry.Any(item => item.RightUrl?.IndexOf("foricadmin", StringComparison.OrdinalIgnoreCase) >= 0
                    || item.RightUrl?.IndexOf("PvWsOrder", StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

    }
}