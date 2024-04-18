using System.Web;

namespace Yahv.Utils.Http
{
    /// <summary>
    /// cookies 帮助者
    /// </summary>
    public class Cookies
    {
        /// <summary>
        /// 支持Cookies操作
        /// </summary>
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

        static object locker1 = new object();

        /// <summary>
        /// CookieHit全局函数
        /// </summary>
        public static CookieHit Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker2)
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

        static Domains domains;

        static object locker2 = new object();

        /// <summary>
        /// Domains全局函数
        /// </summary>
        public static Domains Domain
        {
            get
            {
                if (domains == null)
                {
                    lock (locker2)
                    {
                        if (domains == null)
                        {
                            domains = new Domains();
                        }
                    }
                }
                return domains;
            }
        }
    }
}
