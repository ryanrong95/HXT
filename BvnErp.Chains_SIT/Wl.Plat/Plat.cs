using Needs.Utils.Converters;
using Needs.Utils.Http;
using Needs.Wl.User.Plat.Models;
using System;
using System.Web;
using Cookie = Needs.Wl.User.Plat.Utils.Cookie;

namespace Needs.Wl.User.Plat
{
    /// <summary>
    /// User 入口
    /// </summary>
    public sealed partial class UserPlat
    {
        private UserPlat()
        {

        }

        /// <summary>
        /// 当前用户
        /// </summary>
        static public IPlatUser Current
        {
            get
            {
                IPlatUser user = null;

                if (HttpContext.Current.Session["User"] != null)
                {
                    user = HttpContext.Current.Session["User"] as PlatUser;
                }
                else
                {
                    user = Needs.Wl.User.Plat.Identity.TokenLogin(Token);
                }

                return user;
            }
        }

        private static string Token
        {
            get
            {
                var cookie = Cookie.Current[System.Configuration.ConfigurationManager.AppSettings["Cookie_Name"]];
                if (cookie != null && !string.IsNullOrWhiteSpace(cookie["token"]))
                {
                    return cookie["token"];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="token"></param>
        /// <param name="password"></param>
        public static void ResetPassword(string token, string password)
        {
            var user = Needs.Wl.User.Plat.Identity.EmailTokenLogin(token);
            if (user != null && string.IsNullOrEmpty(user.ID) == false)
            {
                password = password.StrToMD5();
                user.ResetPassword(password);
            }
            else
            {
                throw new Exception("错误的邮件Token");
            }
        }

        /// <summary>
        /// 修改登录名
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newUserName"></param>
        public static void ResetUserName(string newUserName)
        {
            if (string.IsNullOrEmpty(newUserName))
            {
                return;
            }

            Current.ResetUserName(newUserName);
        }
    }
}