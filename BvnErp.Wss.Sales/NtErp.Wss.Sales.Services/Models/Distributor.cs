using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models
{
    /// <summary>
    /// 分销商
    /// </summary>
    public class Distributor : Document
    {
        /// <summary>
        /// 组织机构
        /// </summary>
        public string Organization
        {
            get
            {
                return this[nameof(this.Organization)];
            }
            set
            {
                this[nameof(this.Organization)] = value;
            }
        }

        /// <summary>
        /// 账户名
        /// </summary>
        public string AccountName
        {
            get
            {
                return this[nameof(this.AccountName)];
            }
            set
            {
                this[nameof(this.AccountName)] = value;
            }
        }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank
        {
            get
            {
                return this[nameof(this.Bank)];
            }
            set
            {
                this[nameof(this.Bank)] = value;
            }
        }

        /// <summary>
        /// 迅速代码，该号码相当于各个银行的身份证号码从国外向国内
        /// </summary>
        public string SwiftCode
        {
            get
            {
                return this[nameof(this.SwiftCode)];
            }
            set
            {
                this[nameof(this.SwiftCode)] = value;
            }
        }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account
        {
            get
            {
                return this[nameof(this.Account)];
            }
            set
            {
                this[nameof(this.Account)] = value;
            }
        }
        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress
        {
            get
            {
                return this[nameof(this.BankAddress)];
            }
            set
            {
                this[nameof(this.BankAddress)] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(this.Organization);

            return builder.ToString();
        }


        static public Distributor Default()
        {
            return new Distributor()
            {
                Organization = "IC360 GROUP LIMITED",
                AccountName = "",
                Account = "250-390-16942094(USD)",
                SwiftCode = "CITIHKAX",
                Bank = "Citibank(HongKong)LIMITED",
                BankAddress = "11/F Citi Tower, One Bay East 83 Hoi Bun Road, Kwun Tong, Kowloon HongKong"
            };
        }
    }
}
