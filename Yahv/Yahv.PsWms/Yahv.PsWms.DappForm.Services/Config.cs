using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services
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

        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "localhost:8080";
        public const string DomainName = "sz.warehouse.b1b.com";
        //public const string DomainName = "sz.warehouse.for-ic.net:60077";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/dappapi";

#elif TEST

          /// <summary> 
        /// 域名
        /// </summary>
        public const string SchemeName = "http";

        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "localhost:8080";
        public const string DomainName = "sz.warehouse0.ic360.cn";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/dappapi";
#else
        /// <summary> 
        /// 域名
        /// </summary>
        public const string SchemeName = "http";

        /// <summary>
        /// 域名
        /// </summary>
        //public const string DomainName = "localhost:8080";
        public const string DomainName = "sz.warehouse.for-ic.net:60077";

        /// <summary>
        /// Api接口配置
        /// </summary>
        static public readonly string ApiUrlPrex = $"{SchemeName}://{DomainName}/dappapi";
#endif
    }
}
