using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IClients : Needs.Linq.IUnique, Needs.Linq.IPersistence, Needs.Linq.IFulError, Needs.Linq.IFulSuccess
    {
        string Name { get; }

        string CompanyID { get; }

        IsProtected IsSafe { get; }

        string AdminID { get; }

        ActionStatus Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

        string Summary { get; }

        System.Xml.Linq.XElement Context { get; }

        string NTextString { get; }
    }
}
