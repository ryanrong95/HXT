using System;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 调拨 传输类
    /// </summary>
    public class FundTransferInputDto
    {
        /// <summary>
        /// 调出账号
        /// </summary>
        public string OutAccountNo { get; set; }
        /// <summary>
        /// 调出金额
        /// </summary>
        public decimal OutAmount { get; set; }
        /// <summary>
        /// 调出币制
        /// </summary>
        public string OutCurrency { get; set; }
        /// <summary>
        /// 调出流水号
        /// </summary>
        public string OutSeqNo { get; set; }
        /// <summary>
        /// 调入账号
        /// </summary>
        public string InAccountNo { get; set; }
        /// <summary>
        /// 调入金额
        /// </summary>
        public decimal InAmount { get; set; }
        /// <summary>
        /// 调入币制
        /// </summary>
        public string InCurrency { get; set; }
        /// <summary>
        /// 调入流水号
        /// </summary>
        public string InSeqNo { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 贴现流水号
        /// </summary>
        public string DiscountSeqNo { get; set; }
        /// <summary>
        /// 贴现利息
        /// </summary>
        public decimal? DiscountInterest { get; set; }
        /// <summary>
        /// 手续费流水号
        /// </summary>
        public string PoundageSeqNo { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal? Poundage { get; set; }

        /// <summary>
        /// 手机扫码手续费
        /// </summary>
        public decimal? QRCodeFee { get; set; }

        /// <summary>
        /// 手机扫码手续费 流水号
        /// </summary>
        public string QRCodeFeeSeqNo { get; set; }
    }
}