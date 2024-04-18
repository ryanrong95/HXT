using Needs.Erp.Generic;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NtErp.Crm.Services.Models
{
    public interface IWorksWeekly : IUnique, IPersistence, IFulError, IFulSuccess
    {
        string Context { get; }

        int WeekOfYear { get; }

        string AdminID { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        Status Status { get; }
    }
}
