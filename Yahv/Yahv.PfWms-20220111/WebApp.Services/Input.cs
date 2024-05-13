using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace WebApp.Services
{
    public class Input
    {
        /// <summary>
        /// 全局唯一码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// MainID(订单ID)
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 所属企业
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 销售员ID（admin）
        /// </summary>
        public string SalerID { get; set; }

        /// <summary>
        /// 采购员ID
        /// </summary>
        public string PurchaserID { get; set; }

        /// <summary>
        /// 币种（保值）
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 单价（保值）
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        
    }
}
