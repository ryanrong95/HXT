using System;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 货款
    /// </summary>
    public class PaymentInputDto
    {
        /// <summary>
        /// 付款流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal PayerAmount { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal PayeeAmount { get; set; }
        /// <summary>
        /// 付款币种
        /// </summary>
        public string PayerCurrency { get; set; }
        /// <summary>
        /// 收款币种
        /// </summary>
        public string PayeeCurrency { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 收款人名称
        /// </summary>
        public string PayeeName { get; set; }
        /// <summary>
        /// 收款人银行名称
        /// </summary>
        public string PayeeBankName { get; set; }
        /// <summary>
        /// 收款方账户
        /// </summary>
        public string PayeeAccount { get; set; }
        /// <summary>
        /// 付款方账户
        /// </summary>
        public string PayerAccount { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PaymentDate { get; set; }
    }
}