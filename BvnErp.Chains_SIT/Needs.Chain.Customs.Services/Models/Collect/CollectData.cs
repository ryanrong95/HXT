using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public class CollectData
    {
        public string OrderID { get; set; }
        public string ClientID { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// 外币总额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 已付汇金额
        /// </summary>
        public decimal PaidAmount { get; set; }
        /// <summary>
        /// 已收款金额
        /// </summary>
        public decimal ReceivedAmount { get; set; }
    }

    public class CheckReturnData
    {
        public bool Success { get; set; }
        public string Data { get; set; }
    }

    public class OrderReceiptData
    {
        public string OrderID { get; set; }
        public Enums.FeeType FeeType { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayExchangeData
    {
        public string OrderID { get; set; }
        public string PayExchangeID { get; set; }
        public string PayExchangeItemID { get; set; }
        public decimal Amount { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
