using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IProblem : Needs.Linq.IUnique, Needs.Linq.IPersistence
    {
        string ActionID { get; }

        string StandardID { get; }

        string ReportID { get; }

        string ContactID { get; }

        string Context { get; }

        string Answer { get; }

        string AdminID { get; }

        Status Status { get; }

        DateTime CreateDate { get; }
        DateTime UpdateDate { get; }
    }
}
