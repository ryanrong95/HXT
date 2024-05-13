using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdn.Library.Models
{
    /// <summary>
    /// 快递返回结果
    /// </summary>
    public class KdnResult
    {
        public string EBusinessID { get; set; }
        /// <summary>
        ///面单打印模板
        /// </summary>
        public string PrintTemplate { get; set; }
        /// <summary>
        /// 子模板个数
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        ///子模板单号
        /// </summary>
        public string[] SubOrders { get; set; }
        /// <summary>
        /// 子模板面单
        /// </summary>
        public string[] SubPrintTemplates { get; set; }
        public bool Success { get; set; }
        public string ResultCode { get; set; }
        public string Reason { get; set; }
        public OrderModel Order { get; set; }
        public class OrderModel
        {
            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderCode { get; set; }
            /// <summary>
            /// 快递公司编号
            /// </summary>
            public string ShipperCode { get; set; }
            /// <summary>
            /// 快递单号
            /// </summary>
            public string LogisticCode { get; set; }
            public string DestinatioCode { get; set; }
            public string KDNOrderCode { get; set; }
            public string OriginCode { get; set; }
        }
    }
}
