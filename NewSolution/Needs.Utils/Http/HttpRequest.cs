using System.Web;

namespace Needs.Utils
{
    /// <summary>
    /// HttpRequest
    /// </summary>
    public partial class HttpRequest
    {
        /// <summary>
        /// 获取远程客户端的 IP 主机地址。
        /// </summary>
        public static string UserHostAddress
        {
            get
            {
                //防止使用负载均衡，使用HttpContext.Current.Request.UserHostAddress获取的IP是负载均衡的Web服务器上IP
                string ip = HttpContext.Current.Request.UserHostAddress;
                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // using proxy
                {
                    // Return real client IP.
                    ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    // not using proxy or can't get the Client IP
                    //While it can't get the Client IP, it will return proxy IP.
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }

                return ip;
            }
        }
    }
}