using System;

namespace Needs.Wl.Orders.Services.Models
{
    /// <summary>
    /// 待付汇订单
    /// </summary>
    public class UnPayExchangeOrder : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }
    }
}