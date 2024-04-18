using Needs.Linq;
using System;

namespace Needs.Wl.Client.Services.Models
{
    /// <summary>
    /// 订单付汇记录
    /// </summary>
    public class OrderPayExchangeRecord : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public Needs.Wl.Models.User User { get; set; }

        /// <summary>
        /// Admin
        /// </summary>
        public Needs.Wl.Models.Admin Admin { get; set; }

        /// <summary>
        /// 付汇金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public Needs.Wl.Models.Enums.PayExchangeApplyStatus Status { get; set; }
    }
}