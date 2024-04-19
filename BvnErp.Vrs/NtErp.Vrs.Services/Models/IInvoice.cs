using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public interface IInvoice : Needs.Linq.IUnique
    {
        bool Required { set; get; }
        InvoiceType Type { set; get; }
        string CompanyID { set; get; }
        string ContactID { set; get; }
        string Address { set; get; }
        string Postzip { set; get; }
        string Bank { set; get; }
        string BankAddress { set; get; }
        string Account { set; get; }
        string SwiftCode { set; get; }




    }
}
