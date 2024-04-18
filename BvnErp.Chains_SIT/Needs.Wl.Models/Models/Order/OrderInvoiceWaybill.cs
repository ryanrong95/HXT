using Needs.Linq;
using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单的发票运单
    /// </summary>
    public class OrderInvoiceWaybill : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public Carrier Carrier { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 运输时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
