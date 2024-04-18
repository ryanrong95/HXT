namespace Yahv.Utils.Http
{
    public class iSession
    {
        public object this[int index]
        {
            get { return System.Web.HttpContext.Current.Session[index]; }
            set { System.Web.HttpContext.Current.Session[index] = value; }
        }
        public object this[string index]
        {
            get { return System.Web.HttpContext.Current.Session[index]; }
            set { System.Web.HttpContext.Current.Session[index] = value; }
        }



        static object locker = new object();
        static iSession current = new iSession();

        /// <summary>
        /// CookieHit全局函数
        /// </summary>
        public static iSession Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new iSession();
                        }
                    }
                }
                return current;
            }
        }
    }
}
