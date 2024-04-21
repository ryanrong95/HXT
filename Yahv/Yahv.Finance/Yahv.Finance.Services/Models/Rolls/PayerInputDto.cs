using System;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 付款传输类
    /// </summary>
    public class PayerInputDto
    {
        /// <summary>
        /// 费用类型-固定货款
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 收款方账号
        /// </summary>
        public string InAccountNo { get; set; }
        /// <summary>
        /// 中间账号
        /// </summary>
        public string MidAccountNo { get; set; }
        /// <summary>
        /// 付款账号
        /// </summary>
        public string OutAccountNo { get; set; }
        /// <summary>
        /// RMB总金额
        /// </summary>
        public decimal RMBAmount { get; set; }
        /// <summary>
        /// 外币币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 外币总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 换汇汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 收款流水号
        /// </summary>
        public string InSeqNo { get; set; }
        /// <summary>
        /// 付款流水号
        /// </summary>
        public string OutSeqNo { get; set; }
        /// <summary>
        /// 中间收款流水号
        /// </summary>
        public string MidInSeqNo { get; set; }
        /// <summary>
        /// 中间付款流水号
        /// </summary>
        public string MidOutSeqNo { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }
        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string PoundageSeqNo { get; set; }
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