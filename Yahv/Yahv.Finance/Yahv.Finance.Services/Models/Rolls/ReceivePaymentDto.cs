using System;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 收付款列表
    /// </summary>
    public class ReceivePaymentDto
    {
        /// <summary>
        /// 制单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 对方
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 账户行为
        /// </summary>
        public AccountMethord AccountMethord { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 金库
        /// </summary>
        public string GoldStore { get; set; }
    }
}