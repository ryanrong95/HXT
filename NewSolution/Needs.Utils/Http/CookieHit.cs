using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils.Http
{
    /// <summary>
    /// Http cookies 当前引用
    /// </summary>
    public class CookieHit : ICookieHit
    {
        internal CookieHit()
        {

        }

        string domain;

        internal CookieHit(string domain)
        {
            this.domain = domain;
        }
        public void Remove(string name)
        {
            HttpContext.Current.Request.Cookies.Remove(name);
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

        //DateTime? expires;

        ///// <summary>
        ///// 索引指定名称
        ///// </summary>
        ///// <param name="index">索引指定时间</param>
        ///// <returns>返回当前Hit</returns>
        //public ICookieExpires this[DateTime index]
        //{
        //    get
        //    {
        //        this.expires = index;
        //        return this;
        //    }
        //}

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
    }
}
