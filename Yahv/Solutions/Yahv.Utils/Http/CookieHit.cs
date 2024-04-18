using System;
using System.Web;

namespace Yahv.Utils.Http
{
    /// <summary>
    /// Http cookies 当前引用
    /// </summary>
    public class CookieHit
    {
        internal CookieHit()
        {

        }

        string domain;

        internal CookieHit(string domain)
        {
            this.domain = domain;
        }

        /// <summary>
        /// 索引指定名称
        /// </summary>
        /// <param name="index">索引指定名称</param>
        /// <returns>返回并设置指定索引名称的值</returns>
        public string this[string index]
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[index] == null)
                {
                    return null;
                }

                return HttpContext.Current.Request.Cookies[index].Value;
            }
            set
            {
                lock (this)
                {
                    this.Append(index, value);
                }
            }
        }

        /// <summary>
        /// 追加Cookies
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">生存期</param>
        public void Append(string name, string value, DateTime? expires = null)
        {
            HttpCookie cookie = cookie = new HttpCookie(name);

            //HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            //if (cookie == null)
            //{
            //    cookie = new HttpCookie(name);
            //}

            if (!string.IsNullOrWhiteSpace(this.domain))
            {
                cookie.Domain = this.domain;
            }

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }

            cookie.Value = value;

            HttpContext.Current.Response.AppendCookie(cookie);
        }



        /*
             /// <summary>
        /// 追加Cookies
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">生存期</param>
        public void Append(string name, string value, DateTime? expires = null)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }

            if (!string.IsNullOrWhiteSpace(this.domain))
            {
                cookie.Domain = this.domain;
            }

            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }

            cookie.Value = value;

            HttpContext.Current.Response.AppendCookie(cookie);
        }
         */

    }
}
