using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils.Http
{
    /// <summary>
    /// cookies 帮助者
    /// </summary>
    public class Cookies
    {
        static public bool Supported
        {
            get
            {
                return !(HttpContext.Current == null
                    || HttpContext.Current.Request == null
                    || HttpContext.Current.Request.Cookies == null);
            }
        }

        static CookieHit current;

        static object locker = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        public static CookieHit Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new CookieHit();
                        }
                    }
                }
                return current;
            }
        }

        static public CookieDomain Domain
        {
            get { return CookieDomain.Current; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        Cookies()
        {
            throw new NotSupportedException("类型不支持实例化");
        }

        static CookieHit _cross;
        static object lock_Cross = new object();

        public static CookieHit Cross
        {
            get
            {
                if (_cross == null)
                {
                    lock (lock_Cross)
                    {
                        if (_cross == null)
                        {
                            string[] arry = HttpContext.Current.Request.Url.Host.Split('.'), arry2 = "com,cn".Split(',');
                            string domain = string.Empty;
                            for (int index = arry.Length - 1; index >= 0; index--)
                            {
                                if (!arry2.Contains(arry[index]))
                                {
                                    domain = string.Join(".", arry.Skip(index));
                                    break;
                                }
                            }
                            _cross = new CookieHit(domain);
                        }
                    }
                }
                return _cross;
            }
        }
    }
}
