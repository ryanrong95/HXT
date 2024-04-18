using System;
using System.Configuration;

namespace Yahv.Underly.Attributes
{
    /// <summary>
    /// 接口地址特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ApiAddressAttribute : Attribute
    {
        /// <summary>
        /// 域名地址（从config中获取）
        /// </summary>
        private readonly string hostNameByConfig;

        /// <summary>
        /// 地址
        /// </summary>
        private readonly string address;

        public ApiAddressAttribute(string hostNameByConfig, string address)
        {
            this.hostNameByConfig = hostNameByConfig;
            this.address = address;
        }

        public ApiAddressAttribute(string hostNameByConfig)
        {
            this.hostNameByConfig = hostNameByConfig;
        }

        /// <summary>
        /// 获取接口地址
        /// </summary>
        /// <returns></returns>
        public string GetAddress()
        {
            var hostName = ConfigurationManager.AppSettings[hostNameByConfig];

            if (string.IsNullOrEmpty(hostName))
            {
                throw new Exception($"未找到Config的配置[{hostName}]");
            }

            return $"{hostName}/{address}";
        }

        /// <summary>
        /// 获取Host
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            var hostName = ConfigurationManager.AppSettings[hostNameByConfig];

            if (string.IsNullOrEmpty(hostName))
            {
                throw new Exception($"未找到Config的配置[{hostName}]");
            }

            return hostName;
        }
    }
}