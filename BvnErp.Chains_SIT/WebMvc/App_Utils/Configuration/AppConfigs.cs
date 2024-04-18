using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    public partial class AppConfig
    {
        #region  App.Config、Web.Config中的配置参数

        /// <summary>
        /// 获取文件服务器Url地址
        /// </summary>
        public string FileServerUrl { get; private set; }

        /// <summary>
        /// 获取下载税单请求的路径
        /// </summary>
        public string DownLoadInvoiceUrl { get; private set; }

        /// <summary>
        /// 获取下载报关单请求路径
        /// </summary>
        public string DownLoadDecheadUrl { get; private set; }

        /// <summary>
        /// 获取会员中心Cookie
        /// </summary>
        public string CookieName { get; private set; }

        /// <summary>
        /// 获取Cookie支持的域名
        /// </summary>
        public string CookieDomain { get; private set; }

        #endregion
    }
}