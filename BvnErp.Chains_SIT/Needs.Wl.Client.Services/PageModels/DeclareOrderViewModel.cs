using Needs.Linq;
using System;

namespace Needs.Wl.Client.Services.PageModels
{
    public class DeclareOrderViewModel : IUnique
    {
        /// <summary>
        /// 报关单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 报关总金额
        /// </summary>
        public decimal TotalDeclarePrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
    }
}