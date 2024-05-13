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

        //public const string Web = "http://uuws.b1b.com/";
        public const string Web = "http://221.122.108.49:8198/";

        public const string UploadUrl = Web + "api/Upload";


        /// <summary>
        /// 域名
        /// </summary>
        public const string DomainName = "hv.warehouse.b1b.com";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";
#elif TEST
          /// <summary> 
        /// 域名
        /// </summary>
        public const string SchemeName = "http";

        //public const string Web = "http://uuws.b1b.com/";
        public const string Web = "http://221.122.108.49:8198/";

        public const string UploadUrl = Web + "api/Upload";


        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "warehouse0.ic360.cn";
        public const string DomainName = "221.122.108.49:8199";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";

#else
        /// <summary>
        /// 域名
        /// </summary>
        public const string SchemeName = "http";


        public const string Web = "http://uuws.for-ic.net/";

        public const string UploadUrl = Web + "api/Upload";

        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "warehouse.ic360.cn";
        public const string DomainName = "warehouse.for-ic.net:60077";

        const string ApiUrlName = "erp8.wapi.for-ic.net:60077";
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/wmsapi";
#endif
    }
}
