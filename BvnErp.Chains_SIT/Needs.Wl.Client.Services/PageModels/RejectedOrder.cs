using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class RejectedOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 供应商
        /// </summary>
        public Needs.Wl.Models.ClientSupplier Supplier { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Needs.Wl.Models.Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 退回原因
        /// </summary>
        public string ReturnedSummary { get; set; }
    }
}