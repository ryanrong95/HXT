using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;

namespace Yahv.Erm.Services
{
    public class MyRingRole
    {
        private IErpAdmin admin;
        public MyRingRole(IErpAdmin admin)
        {
            this.admin = admin;

            Init();
        }

        /// <summary>
        /// 菜单
        /// </summary>
        public class Menu
        {
            #region 属性
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 父亲为空时表示系统级；深度为1时表示业务级；深度为2时表示菜单头；深度为3时表示菜单项
            /// </summary>
            public string FatherID { get; set; }

            /// <summary>
            /// 菜单名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 菜单引用的地址
            /// </summary>
            public string RightUrl { get; set; }

            /// <summary>
            /// 标签的地址、css样式
            /// </summary>
            public string IconUrl { get; set; }

            /// <summary>
            /// 首页地址，一般只在业务级别使用
            /// </summary>
            public string FirstUrl { get; set; }

            /// <summary>
            /// Logo地址，一般只在业务级别使用
            /// </summary>
            public string LogoUrl { get; set; }

            /// <summary>
            /// 排序使用，直接在数据库中进行修改即可
            /// </summary>
            public int? OrderIndex { get; set; }

            /// <summary>
            /// 状态：正常、删除
            /// </summary>
            public Status Status { get; set; }

            /// <summary>
            /// HelpUrl地址，一般只在业务级别使用
            /// </summary>
            public string HelpUrl { get; set; }

            public string Company { get; set; }

            #endregion
        }

        Menu[] menus;
        Dictionary<string, string[]> roles;

        void Init()
        {
            using (PvbErmReponsitory reponsitory = new PvbErmReponsitory())
            {
                #region 菜单

                var linq_menus = from entity in reponsitory.ReadTable<Menus>()
                                 where entity.Status == (int)Status.Normal
                                 select new Menu()
                                 {
                                     ID = entity.ID,
                                     Status = (Status)entity.Status,
                                     Name = entity.Name,
                                     OrderIndex = entity.OrderIndex,
                                     IconUrl = entity.IconUrl,
                                     FirstUrl = entity.FirstUrl,
                                     RightUrl = entity.RightUrl,
                                     FatherID = entity.FatherID,
                                     LogoUrl = entity.LogoUrl,
                                     HelpUrl = entity.HelpUrl,
                                     Company = entity.Company,
                                 };

                this.menus = linq_menus.ToArray();

                #endregion

                #region 权限

                var linq_roles = from entity in reponsitory.ReadTable<MapsRole>()
                                 group entity by entity.RoleID into groups
                                 select new
                                 {
                                     RoleID = groups.Key,
                                     MenusID = groups.Select(item => item.MenuID).ToArray()
                                 };

                this.roles = linq_roles.ToDictionary(item => item.RoleID, item => item.MenusID);

                #endregion
            }
        }

        public object this[string business = null]
        {
            get
            {
                IEnumerable<Menu> arry = this.menus;

                //非超级管理员
                if (!this.admin.IsSuper)
                {
                    string[] selects = roles[admin.Role.ID];
                    arry = arry.Where(item => selects.Contains(item.ID));
                }


                var menus = from busi in arry
                            where busi.FatherID == null
                            select new
                            {
                                ID = busi.ID,
                                Name = busi.Name,
                                IconUrl = busi.IconUrl,
                                LogoUrl = busi.LogoUrl,
                                FirstUrl = busi.FirstUrl,
                                HelpUrl = busi.HelpUrl,
                                Company = busi.Company,
                                Menu = (from first in arry
                                        where first.FatherID == busi.ID
                                        select new
                                        {
                                            name = first.Name,
                                            acrivename = first.Name,
                                            router = first.RightUrl,
                                            children = (from second in arry
                                                        where second.FatherID == first.ID
                                                        select new
                                                        {
                                                            name = second.Name,
                                                            acrivename = second.Name,
                                                            router = second.RightUrl
                                                        }).ToArray()
                                        }).ToArray()
                            };

                if (business == null)
                {
                    return menus;
                }
                else
                {
                    return menus.Single(item => item.Name == business).Menu;
                }
            }
        }
    }
}
