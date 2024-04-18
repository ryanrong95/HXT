using Needs.Wl.User.Plat.Models;
using System.Web;

namespace Needs.Wl.User.Plat
{
    /// <summary>
    /// wl.net.cn 微信入口
    /// </summary>
    public sealed partial class WeChatPlat
    {
        static WeChatPlat()
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
                    user = Needs.Wl.User.Plat.WeChatIdentity.OpenIDLogin(OpenID);
                    HttpContext.Current.Session["User"] = user;
                }

                return user;
            }
        }

        /// <summary>
        /// 获取token
        /// </summary>
        public static string OpenID
        {
            get
            {
#if DEBUG
                string currentUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                //TODO:
                if (currentUrl.StartsWith("http://localhost"))
                {
                    return "oYsfd5ykNg1gSmI9LrLUitzcF-Gk";
                }
#endif
                return WeChat.Api.WlOAuth.GetOpenId();
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