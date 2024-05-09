using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services
{
    /// <summary>
    /// 总配置类
    /// </summary>
    /// <remarks>
    /// winform的配置只能开发死在这里
    /// </remarks>
    public class Config
    {
#if DEBUG
        /// <summary> 
        /// 域名
        /// </summary>
        public const string SchemeName = "http";

        public const string Web = "http://uuws.b1b.com/";

        public const string UploadUrl = Web + "api/Upload";


        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "hv.warehouse.b1b.com";

        public const string DomainName = "warehouse0.szhxt.net";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";
#elif TEST
          /// <summary> 
        /// 域名
        /// </summary>
        public const string SchemeName = "http";

        public const string Web = "http://uuws.b1b.com/";

        public const string UploadUrl = Web + "api/Upload";


        /// <summary>
        /// 域名
        /// </summary>
        public const string DomainName = "warehouse0.szhxt.net";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";

#else
        /// <summary>
        /// 域名
        /// </summary>
        public const string SchemeName = "http";


        public const string Web = "http://uuws.ic360.cn/";

        public const string UploadUrl = Web + "api/Upload";

        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "warehouse.ic360.cn";
        public const string DomainName = "warehouse.for-ic.net:60077";

        const string ApiUrlName = "erp8.wapi.for-ic.net:60077";
        //const string ApiUrlName = "api0.szhxt.net";
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";
#endif
    }
}
