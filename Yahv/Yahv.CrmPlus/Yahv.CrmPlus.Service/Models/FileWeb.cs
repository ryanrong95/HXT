using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Yahv.CrmPlus.Service.Models
{
    public class FileHost
    {
        /// <summary>
        /// Web地址配置
        /// </summary>
        static public string Web
        {
            get
            {
                Uri uri = HttpContext.Current.Request.Url;
                return $"{uri.Scheme.ToLower()}://{uri.Host.ToLower()}/_uploader";
            }
        }
    }
}
