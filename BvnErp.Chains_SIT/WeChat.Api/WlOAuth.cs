using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace WeChat.Api
{
    public class WlOAuth
    {
        private static readonly string appID = WebConfigurationManager.AppSettings["appID"];
        private static readonly string appsecret = WebConfigurationManager.AppSettings["appsecret"];

        private const string deskey = "#$%1^&*8";

        public static string GetOpenId()
        {
            string openIdInCookie = GetOpenIdInCookie();
            if (!string.IsNullOrEmpty(openIdInCookie))
            {
                return openIdInCookie;
            }

            return GetNavOpenId();
        }

        /// <summary>
        /// 通过 OAuth 获取 OpenId
        /// </summary>
        private static string GetNavOpenId()
        {
            NameValueCollection parameters = System.Web.HttpContext.Current.Request.Params;

            string navOpenId = string.Empty;

            if (parameters["code"] == null)
            {
                string snsapi_baseUrl = GoCodeUrl();

                CookieHelper.CleanCookie(new[] { CookieHelper.COOKIE_NAME });
                //跳转到微信网页授权页面
                System.Web.HttpContext.Current.Response.Redirect(snsapi_baseUrl, true);
                System.Web.HttpContext.Current.Response.End();
                return null;
            }
            else
            {
                navOpenId = GetRealOpenId(parameters["code"].ToString());

                CookieHelper.CleanCookie(new[] { CookieHelper.COOKIE_NAME });

                Dictionary<string, string> realIdCookie = CookieHelper.GetCookies(new[] { CookieHelper.COOKIE_NAME });
                realIdCookie[CookieHelper.COOKIE_NAME] = DesEncrypt(navOpenId);
                CookieHelper.WriteCookies(realIdCookie, DateTime.MinValue);
            }

            return navOpenId;
        }

        /// <summary>
        /// 从 Cookie 中获取 OpenId
        /// </summary>
        /// <returns></returns>
        private static string GetOpenIdInCookie()
        {
            Dictionary<string, string> cookie = CookieHelper.GetCookies(new[] { CookieHelper.COOKIE_NAME });

            string openId = string.Empty;

            if (cookie != null && cookie.ContainsKey(CookieHelper.COOKIE_NAME))
            {
                string cookieStr = cookie[CookieHelper.COOKIE_NAME];
                if (!string.IsNullOrEmpty(cookieStr))
                {
                    openId = DesDecrypt(cookieStr);
                }
                
            }

            return openId;
        }

        /// <summary>
        /// 网页授权接口第一步
        /// 跳转到获取code的url
        /// </summary>
        /// <returns></returns>
        private static string GoCodeUrl()
        {
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            return OAuthApi.GetAuthorizeUrl(appID, url, "STATE", OAuthScope.snsapi_base);
        }

        /// <summary>
        /// 网页授权接口第二步
        /// 解析code并获取当前访问者真正的openId
        /// </summary>
        /// <param name="parameters">url参数</param>
        /// <returns>真正的openId</returns>
        private static string GetRealOpenId(string code)
        {
            OAuthAccessTokenResult result = OAuthApi.GetAccessToken(appID, appsecret, code);

            string openId = string.Empty;
            if (result != null && !string.IsNullOrEmpty(result.openid))
            {
                openId = result.openid;
            }

            return openId;
        }

        #region 加密解密

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string DesEncrypt(string text)
        {
            string dateStr = DateTime.Now.ToString("T");
            return CommonMethod.DesEncrypt(dateStr + text, deskey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string DesDecrypt(string text)
        {
            string originStr = CommonMethod.DesDecrypt(text, deskey);
            string dateStr = DateTime.Now.ToString("T");
            return originStr.Substring(dateStr.Length, originStr.Length - dateStr.Length);
        }

        #endregion
    }
}
