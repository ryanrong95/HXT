using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.App_Utils
{
    public partial class AppConfig
    {
        #region  AppConfig、WebConfig中的配置参数

        /// <summary>
        /// Purchaser(买方)[HY/XDT]
        /// </summary>
        public string Purchaser { get; private set; }

        /// <summary>
        /// Vendor(卖方)[HT/WLT]
        /// </summary>
        public string Vendor { get; private set; }

        /// <summary>
        /// 获取文件服务器Url地址
        /// </summary>
        public string DomainUrl { get; private set; }

        /// <summary>
        /// 上传路径
        /// </summary>
        public string FileServerUrl { get; private set; }

        /// <summary>
        /// 压缩文件保存路径
        /// </summary>
        public string targetfiledirectory { get; private set; }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailServer { get; private set; }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailUserName { get; private set; }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailDisplayName { get; private set; }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailPassword { get; private set; }

        /// <summary>
        /// 快递面单请求地址
        /// </summary>
        public string KdApiUrl { get; private set; }

        /// <summary>
        /// CRM  API 接口调用
        /// </summary>
        public string CrmUrl { get; private set; }

        #endregion
    }
}