using System;
using System.Collections.Generic;

namespace Needs.Wl.Client.Services.PageModels
{
    public class UnPayExchangeOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 是否可以预换汇，否则不可以在报关前换汇
        /// </summary>
        public bool IsPrePayExchange { get; set; }

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

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public IEnumerable<Needs.Wl.Models.OrderPayExchangeSupplier> PayExchangeSuppliers { get; set; }
    }
}
