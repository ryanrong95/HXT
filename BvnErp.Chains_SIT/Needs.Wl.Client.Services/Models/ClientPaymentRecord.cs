using System;

namespace Needs.Wl.Client.Services.Models
{
    /// <summary>
    /// 客户的付款记录
    /// </summary>
    public class ClientPaymentRecord : Needs.Wl.Models.ReceiptNotice
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款银行账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 收款银行名称
        /// </summary>
        public string AccountBankName { get; set; }

        /// <summary>
        /// 收款银行账号
        /// </summary>
        public string AccountBankAccount { get; set; }
    }
}