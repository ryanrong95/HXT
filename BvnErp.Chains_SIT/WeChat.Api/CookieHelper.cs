using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WeChat.Api
{
    public class CookieHelper
    {
        /// <summary>
        /// 写客户端cookie的名字
        /// </summary>
        public const string COOKIE_NAME = "WeiXinOAuth";

        #region 写COOKIE到客户端

        /// <summary>
        /// 登录后写COOKIE到客户端,代替session
        /// </summary>
        /// <param name="expires">过期时间，如果永不过期，设为DateTime.MaxValue，<para>如果不想写入客户端，浏览器关闭时即失效则设为DateTime.MinValue</para></param>
        /// <param name="values">保存cookie信息</param>
        public static void WriteCookies(Dictionary<string, string> values, DateTime expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[COOKIE_NAME] ?? new HttpCookie(COOKIE_NAME);
            if (expires != DateTime.MinValue)
                cookie.Expires = expires;
            foreach (var value in values)
            {
                cookie.Values[value.Key] = System.Web.HttpUtility.UrlEncode(value.Value);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion

        #region cookie中获取信息

        /// <summary>
        /// cookie中获取信息;
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCookies(string[] cookieKeys)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            HttpCookie cookie = HttpContext.Current.Request.Cookies[COOKIE_NAME];
            if (cookie != null)
            {
                foreach (var cookieKey in cookieKeys)
                {
                    string value = cookie.Values[cookieKey];
                    values.Add(cookieKey,
                               value == null ? null : System.Web.HttpUtility.UrlDecode(cookie.Values[cookieKey].Trim()).Replace("%5F", "_"));
                }
            }
            else
            {
                foreach (var cookieKey in cookieKeys)
                {
                    values.Add(cookieKey, null);
                }
            }
            return values;
        }
        #endregion

        #region 清除cookie

        /// <summary>
        /// 清除cookie
        /// </summary>
        public static void CleanCookie(string[] keys)
        {
            HttpCookie ck = HttpContext.Current.Request.Cookies[COOKIE_NAME];

            if (ck == null || ck.Values.Count == 0)
                return;
            foreach (var key in keys)
            {
                ck.Values[key] = "";
            }
            ck.Expires = DateTime.Now.AddDays(-1.0);
            HttpContext.Current.Response.Cookies.Add(ck);
        }

        #endregion
    }
}
