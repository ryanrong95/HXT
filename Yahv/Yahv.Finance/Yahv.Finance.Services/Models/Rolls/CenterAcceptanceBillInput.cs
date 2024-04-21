using System;
using System.Collections.Generic;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 承兑汇票
    /// </summary>
    public class CenterAcceptanceBillInput
    {
        /// <summary>
        /// 商业承兑、银行承兑
        /// </summary>
        public Enums.MoneyOrderType Type { get; set; }
        /// <summary>
        /// 票据号码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 更新使用
        /// </summary>
        public string OldCode { get; set; }
        /// <summary>
        /// 承兑人信息-全称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 承兑人信息-银行账号
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 承兑人信息-开户行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 承兑人信息-开户行行号
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 汇票金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否允许背书转让
        /// </summary>
        public bool? IsTransfer { get; set; }
        /// <summary>
        /// 是否能贴现
        /// </summary>
        public bool? IsMoney { get; set; }
        /// <summary>
        /// 出票人账号
        /// </summary>
        public string PayerAccountNo { get; set; }
        /// <summary>
        /// 收票人账号
        /// </summary>
        public string PayeeAccountNo { get; set; }
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 汇票到期日
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 承兑性质 电子承兑、纸质承兑
        /// </summary>
        public Enums.MoneyOrderNature Nature { get; set; }
        /// <summary>
        /// 兑换日期
        /// </summary>
        public DateTime? ExchangeDate { get; set; }
        /// <summary>
        /// 兑换金额
        /// </summary>
        public decimal? ExchangePrice { get; set; }
        public Enums.MoneyOrderStatus Status { get; set; }
        public string CreatorID { get; set; }
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 背书人
        /// </summary>
        public string Endorser { get; set; }

        /// <summary>
        /// 费用附件
        /// </summary>
        public List<AcceptanceBillFile> Files { get; set; }

        public class AcceptanceBillFile
        {
            /// <summary>
            /// 附件类型 1 新增附件；
            /// </summary>
            public int FileType { get; set; }
            public string FileName { get; set; }
            public string FileFormat { get; set; }
            public string Url { get; set; }
        }
    }
}