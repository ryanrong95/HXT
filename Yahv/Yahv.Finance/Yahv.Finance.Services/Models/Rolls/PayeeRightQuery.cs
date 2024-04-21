using System;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 核销记录
    /// </summary>
    public class PayeeRightQuery
    {
        public string ID { get; set; }

        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string AccountCatalogName { get; set; }

        /// <summary>
        /// 核销系统
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 核销金额
        /// </summary>
        public decimal RightPrice { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal LeftPrice { get; set; }

        /// <summary>
        /// 核销人
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 付款名称
        /// </summary>
        public string PayerName { get; set; }
    }
}