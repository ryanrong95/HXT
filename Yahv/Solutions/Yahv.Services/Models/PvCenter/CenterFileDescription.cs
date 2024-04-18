using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Linq;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 中心文件状态
    /// </summary>
    public enum FileDescriptionStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        Audting = 100,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,

        /// <summary>
        /// 审批通过
        /// </summary>
        Approved = 300,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 400
    }

    /// <summary>
    /// 中心文件
    /// </summary>
    public class CenterFileDescription : CenterFileMessage, IUnique
    {
        public string ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public FileDescriptionStatus Status { get; set; }
    }

    /// <summary>
    /// 中心文件配置类
    /// </summary>
    public class CenterFile
    {

        /// <summary>
        /// Web地址配置
        /// </summary>
        static public string Web
        {
            get
            {

                string config = System.Configuration.ConfigurationManager.AppSettings["Uploader"];
                if (!string.IsNullOrWhiteSpace(config))
                {
                    return config;
                }

                //var tests = new[] { "v1.for-ic.net", "user1.for-ic.net", "client0.ic360.cn", "erp80.ic360.cn" };
                var tests = new[] { "v1.for-ic.net", "user1.for-ic.net", "client0.foric.b1b.cn", "erp80.foric.b1b.cn" };
                //var official = new[] { "www.for-ic.net", "user.for-ic.net", "erp8.ic360.cn" };
                var official = new[] { "www.for-ic.net", "user.for-ic.net", "erp8.for-ic.net" };

                if (HttpContext.Current != null
                    && HttpContext.Current.Request != null
                    && HttpContext.Current.Request.Url != null)
                {
                    string host = HttpContext.Current.Request.Url.Host.ToLower();
                    //if (tests.Contains(host))
                    if (tests.Any(item => host.Contains(item)))
                    {
                        //return "http://uuws.b1b.com/";
                        return "http://221.122.108.49:8198/";
                    }

                    //if (official.Contains(host))
                    if (official.Any(item => host.Contains(item)))
                    {
                        //return "http://uuws.ic360.cn/";
                        return "http://uuws.for-ic.net/";
                    }
                }


#if DEBUG
                //return "http://uuws.b1b.com/";
                return "http://221.122.108.49:8198/";
#elif TEST
                //return "http://uuws.b1b.com/";
                return "http://221.122.108.49:8198/";
#else
                //return "http://uuws.ic360.cn/";
                return "http://uuws.for-ic.net/";
#endif

            }
        }

        //#if DEBUG
        //        public const string Web = "http://uuws.b1b.com/";
        //#elif TEST
        //        public const string Web = "http://uuws.b1b.com/";
        //#else
        //        public const string Web = "http://uuws.ic360.cn/";
        //#endif
    }
}
