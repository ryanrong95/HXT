using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSApis.Models
{
    public class UrlConfig
    {
        public string ApiServerUrl { get; set; }

        /// <summary>
        /// 账户余额查询地址
        /// </summary>
        public string ABEUrl { get; set; }

        /// <summary>
        /// 报价查询地址
        /// </summary>
        public string FXPricingUrl { get; set; }

        /// <summary>
        /// 锁定汇率地址
        /// </summary>
        public string FXBookingUrl { get; set; }

        /// <summary>
        /// 购汇地址
        /// </summary>
        public string ACTUrl { get; set; }

        /// <summary>
        /// 账户流水查询
        /// </summary>
        public string AREUrl { get; set; }

        /// <summary>
        /// 境内人民币付款
        /// </summary>
        public string CNAPSUrl { get; set; }

        /// <summary>
        /// 境外汇款
        /// </summary>
        public string TTUrl { get; set; }
    }
}