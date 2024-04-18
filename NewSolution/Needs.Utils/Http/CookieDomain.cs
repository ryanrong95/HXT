using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils.Http
{

    /// <summary>
    /// Http cookies 域引用
    /// </summary>
    public class CookieDomain
    {
        ConcurrentDictionary<string, CookieHit> dic;

        CookieDomain()
        {
            this.dic = new ConcurrentDictionary<string, Http.CookieHit>();
        }

        /// <summary>
        /// 索引指定域
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>返回指定域的 Cookies</returns>
        public CookieHit this[string index]
        {
            get
            {
                return this.dic.GetOrAdd(index, new CookieHit(index));
            }
        }

        static CookieDomain current;

        static object locker = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        static internal CookieDomain Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new CookieDomain();
                        }
                    }
                }
                return current;
            }
        }
    }
}
