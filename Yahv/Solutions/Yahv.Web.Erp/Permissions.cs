using System;
using System.Linq;
using Yahv.Models;

namespace Yahv.Web.Erp
{
    /// <summary>
    /// 权限
    /// </summary>
    class Permissions
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <param name="settings">菜单地址、颗粒化</param>
        /// <param name="url">当前地址</param>
        /// <returns></returns>
        public static bool IsHavePermission(PermissionSetting settings, string url)
        {
            if (settings.MyMenes == null || settings.MyMenes.Length <= 0)
                return false;

            Uri uri = new Uri(url);
            string path = string.Join("", uri.Segments.Take(uri.Segments.Length - 1));
            string pathFull = string.Join("", uri.Segments.Take(uri.Segments.Length));


            //例如没有分配的详情页面，默认为可访问
            if (!settings.MenesAll.Contains(pathFull))
            {
                return true;
            }

            return settings.MyMenes.Where(item => !string.IsNullOrWhiteSpace(item))
                       .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0)
                   ||
                   settings.ParticleSettings.Where(item => !string.IsNullOrWhiteSpace(item))
                       .Any(item => item.Equals(url, StringComparison.OrdinalIgnoreCase)
                                    || item.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}