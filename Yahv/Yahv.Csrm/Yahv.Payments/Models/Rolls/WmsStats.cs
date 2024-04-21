using System;
using Yahv.Underly;

namespace Yahv.Payments.Models
{
    /// <summary>
    /// 库房统计
    /// </summary>
    public class WmsStats
    {
        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 收款公司ID
        /// </summary>
        public string Payee { get; set; }
        public string PayerName { get; set; }
        public string PayeeName { get; set; }
        public Currency Currency { get; set; }
        public string CurrencyName { get; set; }
        public string EnterCode { get; set; }
        public decimal? LeftPrice { get; set; }
        public decimal? RightPrice { get; set; }
        public string CreateDate { get; set; }
        public string OrderID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        public string Catalog { get; set; }
        public string Subject { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 库房明细
    /// </summary>
    public class WmsDetail
    {
        public string ID { get; set; }

        /// <summary>
        /// 记账、现金
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款公司ID
        /// </summary>
        public string Payee { get; set; }

        public string PayerName { get; set; }
        public string PayeeName { get; set; }
        public Currency Currency { get; set; }
        public string CurrencyName { get; set; }
        public string EnterCode { get; set; }
        public decimal? LeftPrice { get; set; }
        public decimal? RightPrice { get; set; }
        public string CreateDate { get; set; }
        public string OrderID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        public string Catalog { get; set; }
        public string Subject { get; set; }
        public string CreatorName { get; set; }
        public string summary { get; set; }
    }
}