using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Utils.Http
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
                HttpCookie cookie = HttpContext.Current.Request.Cookies[index];
                if (cookie == null)
                {
                    cookie = new HttpCookie(index);
                }
                cookie.Expires = DateTime.Now.AddDays(20);
                cookie.Value = value;

                if (!string.IsNullOrWhiteSpace(this.domain))
                {
                    cookie.Domain = this.domain;
                }
                //else
                //{
                //    string host = HttpContext.Current.Request.Url.Host;
                //    var arry = host.Split('.');
                //    cookie.Domain = $"{(arry.Length <= 2 ? "." + host : "." + arry[arry.Length - 2] + "." + arry[arry.Length - 1])}";
                //}
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }
    }

}
