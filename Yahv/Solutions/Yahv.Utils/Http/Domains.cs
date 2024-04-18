namespace Yahv.Utils.Http
{
    /// <summary>
    /// Cookies域
    /// </summary>
    public class Domains
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="domain">指定域</param>
        /// <returns>CookieHit</returns>
        public CookieHit this[string domain]
        {
            get { return new CookieHit(domain); }
        }

        internal Domains()
        {

        }
    }
}
