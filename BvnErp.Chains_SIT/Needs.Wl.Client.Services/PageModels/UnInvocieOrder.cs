using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class UnInvocieOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 开票类型
        /// </summary>
        public Needs.Wl.Models.Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 报关状态
        /// </summary>
        public int DeclareStatus { get; set; }

        /// <summary>
        /// 付汇状态
        /// </summary>
        public Needs.Wl.Models.Enums.PayExchangeStatus PayExchangeStatus
        {
            get
            {
                if (this.PaidExchangeAmount == 0)
                {
                    return Needs.Wl.Models.Enums.PayExchangeStatus.UnPay;
                }
                else if (this.PaidExchangeAmount < DeclarePrice)
                {
                    return Needs.Wl.Models.Enums.PayExchangeStatus.Partial;
                }
                else
                {
                    return Needs.Wl.Models.Enums.PayExchangeStatus.All;
                }
            }
        }
    }
}