using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils
{
    public class URI
    {
        static object lockHelp = new object();

        /// <summary>
        /// 统一资源标识方案
        /// </summary>
        public static string Scheme
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    var scheme = System.Configuration.ConfigurationManager.AppSettings.Get("RequestScheme");
                    if (string.IsNullOrEmpty(scheme))
                    {
                        scheme = "http";
                    }
                    return scheme;
                }
                else
                {
                    return HttpContext.Current.Request.Url.Scheme;
                }
            }
        }
    }
}
