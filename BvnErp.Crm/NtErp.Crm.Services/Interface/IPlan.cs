using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using System.Collections;
using System.Linq.Expressions;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public interface IPlan : Needs.Linq.IUnique
    {
        string Name { get;  }
        string ClientID { get;}
        string CompanyID { get;}
        string CatalogueID { get;}
        string SaleID { get; }
        string SaleManagerID { get;}
        ActionTarget Target { get;}
        ActionMethord Methord { get;}
        string AdminID { get; }
        DateTime PlanDate { get;}
        DateTime? StartDate { get;}
        DateTime? EndDate { get; }

        ActionStatus Status { get;}

        DateTime CreateDate { get; }

        DateTime UpdateDate { get;}

        string Summary { get; }
    }
}
