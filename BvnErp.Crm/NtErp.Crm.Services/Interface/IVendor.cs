using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IVendor : IUnique, IPersistence
    {
        CompanyType ObjectType { get; }

        string Name { get; }

        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }
        
        string PM { get; }

        string FAE { get; }
    }
}
