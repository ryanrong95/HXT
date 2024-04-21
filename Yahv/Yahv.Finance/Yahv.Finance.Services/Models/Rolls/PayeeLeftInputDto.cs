using System;

namespace Yahv.Finance.Services.Models
{
    /// <summary>
    /// 收款
    /// </summary>
    public class PayeeLeftInputDto
    {
        /// <summary>
        /// 收款类型（dbo.AccountCatalogs）
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 收款方式
        /// </summary>
        /// <see cref="Yahv.Underly.PaymentMethord"/>
        public int ReceiptType { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 旧流水号
        /// </summary>
        /// <remarks>修改的话，用这个来查询要修改记录</remarks>
        public string OldSeqNo { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 账户性质
        /// </summary>
        /// <see cref="Yahv.Underly.NatureType"/>
        public int AccountSource { get; set; }
    }
}