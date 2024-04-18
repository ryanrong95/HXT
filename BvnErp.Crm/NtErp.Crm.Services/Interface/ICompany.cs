using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using Needs.Linq;

namespace NtErp.Crm.Services.Models
{
    public interface ICompany : IUnique,IPersistence,IFulError,IFulSuccess
    {
        string Name { get; }

        string Code { get;}

        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        string Summary { get; }
    }
}
