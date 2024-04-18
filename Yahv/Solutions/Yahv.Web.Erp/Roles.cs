using System.Linq;
using Layers.Data.Sqls;
using System;
using Yahv.Underly.Erps;

namespace Yahv.Web.Erp
{
    /// <summary>
    /// 角色
    /// </summary>
    /// <remarks>临时开发后期修改</remarks>
    class Roles
    {
        /// <summary>
        /// 判断权限下是否存在可访问地址
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <remarks>临时开发后期修改</remarks>
        static public bool Contains(string roleID, string url)
        {
            using (PvbErmReponsitory reponsitory = new PvbErmReponsitory())
            {
                var linq = from map in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRole>()
                           join menu in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>() on map.MenuID equals menu.ID
                           where map.RoleID == roleID
                           select menu.RightUrl;
                string[] urls = linq.ToArray();


                var linqSettings = from settings in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                                   where settings.RoleID == roleID
                                   select settings.Url;
                string[] setingurls = linqSettings.ToArray();

                //增加如果是指定Url地址下的那些

                Uri uri = new Uri(url);
                //string path = string.Join("", uri.Segments.Take(uri.Segments.Length - 1));
                string path = string.Join("", uri.Segments.Take(uri.Segments.Length));

                return urls.Where(item => !string.IsNullOrWhiteSpace(item))
                    .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                    ||
                    setingurls.Where(item => !string.IsNullOrWhiteSpace(item))
                    .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0
                    );
            }
        }

        static public bool Contains(IRole role, string url)
        {
            using (PvbErmReponsitory reponsitory = new PvbErmReponsitory())
            {
                var linq = from map in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRole>()
                           join menu in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>() on map.MenuID equals menu.ID
                           where map.RoleID == role.ID
                           select menu.RightUrl;
                string[] urls = linq.ToArray();


                var linqSettings = from settings in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                                   where settings.RoleID == role.ID
                                   select settings.Url;
                string[] setingurls = linqSettings.ToArray();

                //增加如果是指定Url地址下的那些
                Uri uri = new Uri(url);
                string path = string.Join("", uri.Segments.Take(uri.Segments.Length));

                return urls.Where(item => !string.IsNullOrWhiteSpace(item))
                    .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0
                    )
                    ||
                    setingurls.Where(item => !string.IsNullOrWhiteSpace(item))
                    .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0
                    );
            }
        }
    }
}