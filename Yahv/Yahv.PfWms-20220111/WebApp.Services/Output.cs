using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace WebApp.Services
{
    /// <summary>
    /// 销项信息
    /// </summary>
    public class Output
    {
    
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string OwnerID { get; set; }

        /// <summary>
        /// 销售人员
        /// </summary>
        public string SalerID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string CustomerServiceID{get;set;}
    
        /// <summary>
        /// 采购员
        /// </summary>
        public string PurchaserID { get; set; }

        /// <summary>
        /// 币种(保值)
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 价格(保值)
        /// </summary>
        public decimal? Price { get; set; }
    }
}
