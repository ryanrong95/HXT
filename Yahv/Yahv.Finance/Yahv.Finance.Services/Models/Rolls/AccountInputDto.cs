namespace Yahv.Finance.Services.Models
{
    public class AccountInputDto
    {
        /// <summary>
        /// 金库名称
        /// </summary>
        public string VaultName { get; set; }
        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string BankAccount { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 币制（CNY）
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 账号类型
        /// </summary>
        public string AccountType { get; set; }
        /// <summary>
        /// 账号管理人
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 地区（CHN）
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }

        /// <summary>
        /// 账户来源 1标准 2简易
        /// </summary>
        public int AccountSource { get; set; }

        /// <summary>
        /// 是否虚拟
        /// </summary>
        public bool IsVirtual { get; set; }
    }
}