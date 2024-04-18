using Needs.Erp.Generic;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IStandardProduct
    {
        string Origin { get; }

        string Name { get; }

        string ManufacturerID { get; }

        string Packaging { get; }

        string PackageCase { get; }

        string Batch { get; }

        string DateCode { get; }

    }
}
