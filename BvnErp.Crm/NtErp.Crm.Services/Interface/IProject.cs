using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IProject : Needs.Linq.IUnique,Needs.Linq.IPersistence
    {
        string Name { get;  }

        string ClientID { get; }

        string CompanyID { get; }

        decimal? Valuation { get; }

        CurrencyType Currency { get; }

        string CatelogueID { get; }

        string AdminID { get; }

        string Summary { get; }

        DateTime? StartDate { get; }

        DateTime? EndDate { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        ActionStatus Status { get; }
    }
}
