#define DEVELOP     //开发
//#define TEST      //测试版
//#define FINAL     //生产版  生产版的配置严格禁止修改、删除，可以添加


namespace Yahv.Underly
{
    /// <summary>
    /// 域名
    /// </summary>
    //    public enum FromType
    //    {
    //#if DEVELOP
    //        #region Yahv.Erp

    //        /// <summary>
    //        /// 主域
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com")]
    //        Erp = 1,
    //        /// <summary>
    //        /// 询报价
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/faqs")]
    //        FAQs = 2,
    //        /// <summary>
    //        /// CRM
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/crms")]
    //        Crm = 3,

    //        #endregion
    //#elif TEST
    //        #region Yahv.Erp

    //        /// <summary>
    //        /// 主域
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com")]
    //        Erp = 1,
    //        /// <summary>
    //        /// 询报价
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/faqs")]
    //        FAQs = 2,
    //        /// <summary>
    //        /// CRM
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/crms")]
    //        Crm = 3,

    //        #endregion
    //#elif FINAL
    //        #region Yahv.Erp

    //        /// <summary>
    //        /// 主域
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com")]
    //        Erp = 1,
    //        /// <summary>
    //        /// 询报价
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/faqs")]
    //        FAQs = 2,
    //        /// <summary>
    //        /// CRM
    //        /// </summary>
    //        [Description("http://Yahv.Erp.b1b.com/crms")]
    //        Crm = 3,

    //        #endregion
    //#endif
    //    }

    /// <summary>
    /// 域名配置
    /// </summary>
    static public class DomainConfig
    {
        /// <summary>
        /// 主域
        /// </summary>
        static public string Domain
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["domain"];
            }
        }

        /// <summary>
        /// 询报价
        /// </summary>
        static public string FAQs
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["faqs"];
            }
        }

        /// <summary>
        /// CRM
        /// </summary>
        static public string Crm
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["crm"];
            }
        }
        /// <summary>
        /// 资源文件
        /// </summary>
        static public string Fixed
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["fixed"];
            }
        }

        /// <summary>
        /// 资源文件版本
        /// </summary>
        static public string FixedVersion
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["fixedVersion"];
            }
        }

        /// <summary>
        /// Erp Api
        /// </summary>
        static public string ErmApi
        {
            get
            {
#if DEBUG
                return "http://hv.erp.b1b.com/ermapi/";
#else


                return System.Configuration.ConfigurationManager.AppSettings["ermapi"];
#endif
            }
        }
        /// <summary>
        /// 标准库
        /// </summary>
        static public string StandardHost
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["standardhost"];
            }
        }
    }
}
