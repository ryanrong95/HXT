using Needs.Utils;
using Needs.Utils.Http;
using Needs.Wl.Models;
using Needs.Wl.User.Plat.Models;
using System;
using System.Text;
using System.Web;
using HttpRequest = Needs.Utils.HttpRequest;

namespace Needs.Wl.User.Plat
{
    /// <summary>
    /// UserPlat 身份验证
    /// </summary>
    public static class Identity
    {
        /// <summary>
        /// 邮箱Token验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static EmailToken EmailsToken(string token)
        {
            //验证token
            token = token.InputText();
            return new Views.EmailTokenView(token).FirstOrDefault();
        }

        /// <summary>
        /// token 邮箱验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IPlatUser EmailTokenLogin(string token)
        {
            return new Views.UserEmailTokenLogin(token.InputText()).FirstOrDefault();
        }

        /// <summary>
        /// 通过邮箱查找user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static IPlatUser FindUserByEmail(string email)
        {
            return new Views.UsersEmail(email.InputText()).FirstOrDefault();
        }

        /// <summary>
        /// 使用用户名密码登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IPlatUser UserLogin(string userName, string password)
        {
            return new Views.UsersLogin(userName.InputText(), password.InputText()).FirstOrDefault();
        }

        /// <summary>
        /// 使用Token登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IPlatUser TokenLogin(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            token = token.InputText();
            string ip = HttpRequest.UserHostAddress;
            return new Views.UserTokenLogin(token, ip).FirstOrDefault();
        }

        /// <summary>
        /// 使用用户名+Cookie中的Token进行身份验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IPlatUser TokenLogin(string userName, string token)
        {
            string ip = HttpRequest.UserHostAddress;
            return new Views.UserTokenLogin(userName.InputText(), token.InputText(), ip).FirstOrDefault();
        }

        public static void CreateUserToken(IPlatUser user, string token, string ip)
        {
            Needs.Wl.Models.UserToken entity = new Needs.Wl.Models.UserToken();
            entity.UserID = user.ID;
            entity.Token = token;
            entity.IP = ip;
            entity.Enter();
        }

        public static void ResponseCookie(string name, string token, bool remberMe)
        {
            if (Cookies.Supported)
            {
                string cookieName = System.Configuration.ConfigurationManager.AppSettings["Cookie_Name"];

                HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
                if (cookie == null)
                {
                    cookie = new HttpCookie(cookieName);
                }

                cookie.Values["token"] = token;
                cookie.Values["name"] = HttpUtility.UrlEncode(name, Encoding.GetEncoding("UTF-8"));
                cookie.Values["isRem"] = remberMe.ToString();

#if !DEBUG
                    cookie.Domain =  System.Configuration.ConfigurationManager.AppSettings["Domain"]; 
#endif

                cookie.Expires = DateTime.Now.AddDays(30);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }

        public static void ClearCookie()
        {
            string cookieName = System.Configuration.ConfigurationManager.AppSettings["Cookie_Name"];

            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null && cookie.Values["isRem"].ToLower() == "false")
            {
#if !DEBUG
                    cookie.Domain =  System.Configuration.ConfigurationManager.AppSettings["Domain"]; 
#endif
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }
    }
}