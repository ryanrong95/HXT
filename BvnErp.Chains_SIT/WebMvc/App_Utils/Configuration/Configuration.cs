using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    /// <summary>
    /// 系统配置
    /// 读取AppConfig、WebConfig中的配置参数
    /// AppConfig(WebConfig)设置说明：
    /// </summary>
    public partial class AppConfig
    {
        static object locker = new object();
        static AppConfig current;

        public static AppConfig Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new AppConfig();
                        }
                    }
                }

                return current;
            }
        }

        private AppConfig()
        {
            this.FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            this.DownLoadInvoiceUrl = System.Configuration.ConfigurationManager.AppSettings["DownLoadInvoiceUrl"];
            this.DownLoadDecheadUrl = System.Configuration.ConfigurationManager.AppSettings["DownLoadDecheadUrl"];
            this.CookieName = System.Configuration.ConfigurationManager.AppSettings["Cookie_Name"];
            this.CookieDomain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
        }
    }
}