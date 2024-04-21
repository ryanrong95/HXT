using System;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 认领表传输类
    /// </summary>
    public class AccountWorkDto
    {
        public string ID { get; set; }

        /// <summary>
        /// 收款ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 认领人ID
        /// </summary>
        public string ClaimantName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 收款类型名称
        /// </summary>
        public string AccountCatalogName { get; set; }

        /// <summary>
        /// 收款公司
        /// </summary>
        public string AccountEnterprise { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string AccountCode { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 币种描述
        /// </summary>
        public string CurrencyDesc => Currency.GetDescription();

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        public string AccountShortName { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }
    }
}