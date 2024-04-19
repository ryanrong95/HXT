using Needs.Underly;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public interface IBeneficiary : Needs.Linq.IUnique
    {
        string ID { set; get; }
        string Bank { set; get; }
        PayMethod Method { set; get; }
        Currency Currency { set; get; }
        string Address { set; get; }
        string SwiftCode { set; get; }
        string ContactID { set; get; }
        string CompanyID { set; get; }
        Status Status { set; get; }
    }
}
