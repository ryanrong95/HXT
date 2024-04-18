using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.App_Utils
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
            this.Purchaser = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
            this.Vendor = System.Configuration.ConfigurationManager.AppSettings["Vendor"];
            this.DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
            this.FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            this.targetfiledirectory = System.Configuration.ConfigurationManager.AppSettings["targetfiledirectory"];
            this.MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"];
            this.MailUserName = System.Configuration.ConfigurationManager.AppSettings["MailUserName"];
            this.MailDisplayName = System.Configuration.ConfigurationManager.AppSettings["MailDisplayName"];
            this.MailPassword = System.Configuration.ConfigurationManager.AppSettings["MailPassword"];
            this.KdApiUrl = System.Configuration.ConfigurationManager.AppSettings["KdApiUrl"];
            this.CrmUrl = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        }
    }
}