using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Wss.Sales.Services.Utils.Http
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

        static CookieHit _current;

        static object lock_Current = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        public static CookieHit Current
        {
            get
            {
                if (_current == null)
                {
                    lock (lock_Current)
                    {
                        if (_current == null)
                        {
                            _current = new CookieHit();
                        }
                    }
                }
                return _current;
            }
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

        static CookieDomain _Domain;

        static object lock_Domain = new object();

        /// <summary>
        /// 域引用
        /// </summary>
        public static CookieDomain Domains
        {
            get
            {
                if (_Domain == null)
                {
                    lock (lock_Domain)
                    {
                        if (_Domain == null)
                        {
                            _Domain = new CookieDomain();
                        }
                    }
                }
                return _Domain;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        Cookies()
        {
            throw new NotSupportedException("类型不支持实例化");
        }
    }


}
