using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    /// <summary>
    /// 用户发票类型（1 普票、2 增票）
    /// </summary>
    public enum InvoiceType
    {
        /// <summary>
        /// 普票
        /// </summary>

        Invoice = 1,
        /// <summary>
        /// 增票
        /// </summary>

        VAT = 2
    }

    public class Invoice : Needs.Linq.IUnique
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string CompanyName { get; set; }
        public string SCC { get; set; }
        public string RegAddress { get; set; }
        public string Tel { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public InvoiceType Type { get; set; }


        public NtErp.Wss.Oss.Services.Models.Invoice ToOss(Consignee consignee)
        {
            var company = new NtErp.Wss.Oss.Services.Models.Company
            {
                Type = NtErp.Wss.Oss.Services.CompanyType.Company,
                Address = this.RegAddress,
                Code = this.SCC,
                Name = this.CompanyName,
            };

            return new NtErp.Wss.Oss.Services.Models.Invoice
            {
                Account = this.BankAccount,
                Bank = this.BankName,
                BankAddress = "",
                Contact = new NtErp.Wss.Oss.Services.Models.Contact
                {
                    Email = consignee.Email,
                    Company = new NtErp.Wss.Oss.Services.Models.Company
                    {
                        Type = NtErp.Wss.Oss.Services.CompanyType.Company,
                        Address =  consignee.Address,
                        Code = "",
                        Name = consignee.Company,
                    },
                    Mobile = "",
                    Name = consignee.FirstName + consignee.LastName,
                    Tel = consignee.Tel,
                },
                Address = "",
                Company = company,
                Postzip = consignee.Zipcode,
                Required = true,
                SwiftCode = "",
                Type = (NtErp.Wss.Oss.Services.Models.InvoiceType)(int)this.Type,

            };
        }
    }
}
