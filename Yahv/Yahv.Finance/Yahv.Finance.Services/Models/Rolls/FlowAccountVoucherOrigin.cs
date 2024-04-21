using System;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 本位币对账单
    /// </summary>
    public class FlowAccountVoucher
    {
        /// <summary>
        /// 流水ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 往来单位
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 账户行为
        /// </summary>
        public AccountMethord AccountMethord { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal LeftPrice { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 收款本位币金额
        /// </summary>
        public decimal LeftPrice1 { get; set; }

        /// <summary>
        /// 支出金额
        /// </summary>
        public decimal RightPrice { get; set; }

        /// <summary>
        /// 支出本位币金额
        /// </summary>
        public decimal RightPrice1 { get; set; }

        /// <summary>
        /// 发生余额
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 本位币余额
        /// </summary>
        public decimal? Balance1 { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
    }
}