using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 银行流水
    /// </summary>
    public class BankFlow
    {
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        public BankFlow(string bank, string account, string formCode)
        {
            this.Bank = bank;
            this.Account = account;
            this.FormCode = formCode;
        }
    }
}
