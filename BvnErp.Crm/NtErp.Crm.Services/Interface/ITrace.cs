using Needs.Linq;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    interface ITrace : IUnique, IPersistence, IFulError, IFulSuccess
    {
        string ClientID { get; }
        ActionMethord Type { get; }
        DateTime Date { get; }
        string Context { get; }
        string AdminID { get; }
        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }

    }
}
