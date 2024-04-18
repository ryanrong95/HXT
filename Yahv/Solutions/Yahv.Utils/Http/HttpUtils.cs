using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.Http
{
    public class HttpUtils
    {
        /// <summary>
        /// 字符串
        /// </summary>
        static public NameValueCollection RequestQuery
        {
            get
            {
                return System.Web.HttpContext.Current.Request.QueryString;
            }
        }

        /// <summary>
        /// Form
        /// </summary>
        static public NameValueCollection RequestForm
        {
            get
            {
                return System.Web.HttpContext.Current.Request.Form;
            }
        }
    }
}

