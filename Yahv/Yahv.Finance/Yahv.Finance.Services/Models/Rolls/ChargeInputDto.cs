using System;
using System.Collections.Generic;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 费用申请 传输类
    /// </summary>
    public class ChargeInputDto
    {
        /// <summary>
        /// 收款方账号
        /// </summary>
        public string ReceiveAccountNo { get; set; }

        /// <summary>
        /// 付款账户
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 旧流水号
        /// </summary>
        public string OldSeqNo { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// 银行自动扣除 1否 2是
        /// </summary>
        public int MoneyType { get; set; }
        /// <summary>
        /// 费用项目
        /// </summary>
        public List<ChargeItemDto> FeeItems { get; set; }

        /// <summary>
        /// 费用附件
        /// </summary>
        public List<CenterFeeFile> Files { get; set; }
    }

    /// <summary>
    /// 费用申请项
    /// </summary>
    public class ChargeItemDto
    {
        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 费用描述
        /// </summary>
        public string FeeDesc { get; set; }
    }

    public class CenterFeeFile
    {
        /// <summary>
        /// 附件类型 1 费用申请附件；2 费用支付凭证
        /// </summary>
        public int FileType { get; set; }
        public string FileName { get; set; }
        public string FileFormat { get; set; }
        public string Url { get; set; }
    }

}