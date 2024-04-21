using System;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 货款对账单
    /// </summary>
    public class PayerStatement
    {
        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }
        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }
        /// <summary>
        /// 付款分类
        /// </summary>
        public string AccountCatalogID { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 应付ID
        /// </summary>
        public string LeftID { get; set; }
        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal LeftPrice { get; set; }
        /// <summary>
        /// 应付时间
        /// </summary>
        public DateTime LeftDate { get; set; }
        /// <summary>
        /// 实付金额
        /// </summary>
        public decimal? RightPrice { get; set; }
        /// <summary>
        /// 最近一次实付时间
        /// </summary>
        public DateTime? RightDate { get; set; }
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
    }
}