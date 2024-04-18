using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class UnComfirmOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 开票类型
        /// </summary>
        public Needs.Wl.Models.Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }
    }
}