using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Wl.User.Plat.Utils
{
    public class Cookie
    {
        static Cookie current;
        static object locker = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        static public Cookie Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Cookie();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            HttpContext.Current.Request.Cookies.Remove(name);
        }

        /// <summary>
        /// 索引指定名称
        /// </summary>
        /// <param name="index">索引指定名称</param>
        /// <returns>返回并设置指定索引名称的值</returns>
        public NameValueCollection this[string index]
        {
            get
            {
                if (HttpContext.Current.Request.Cookies[index] == null)
                {
                    return null;
                }

                return HttpContext.Current.Request.Cookies[index].Values;
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
        /// 添加cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <param name="expires"></param>
        /// <param name="domain"></param>
        public void Append(string name, NameValueCollection values, DateTime? expires = null, string domain = null)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }

            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    cookie.Values[key] = values[key];
                }
            }
     
            if (!string.IsNullOrWhiteSpace(domain))
            {
                cookie.Domain = domain;
            }
            if (expires.HasValue)
            {
                cookie.Expires = expires.Value;
            }

            HttpContext.Current.Response.AppendCookie(cookie);
        }
    }
}