using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Utils.Http
{

    /// <summary>
    /// Http cookies 域引用
    /// </summary>
    public class CookieDomain
    {
        /// <summary>
        /// 索引指定域
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>返回指定域的 Cookies</returns>
        public CookieHit this[string domain]
        {
            get { return new CookieHit(domain); }
        }
    }
}
