using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterAccount
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
        /// 币制
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
        /// 国家地区
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 操作ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance { get; set; }
        /// <summary>
        /// 账号来源 1-标准 2- 简易
        /// </summary>
        public int AccountSource { get; set; }
    }
}
