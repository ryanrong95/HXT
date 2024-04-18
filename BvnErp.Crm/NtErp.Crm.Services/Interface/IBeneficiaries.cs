using Needs.Erp.Generic;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IBeneficiaries : IUnique, IPersistence, IFulError, IFulSuccess
    {
        string Bank { get; }

        string BankCode{get; }
        string Address { get; }
        string CompanyID { get; }
        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }
    }
}
