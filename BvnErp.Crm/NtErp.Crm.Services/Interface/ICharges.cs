using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface ICharges : Needs.Linq.IUnique
    {
        string ClientID { get;  }

        string ActionID { get; }

        string Name { get; }

        string AdminID { get; }

        int? Count { get; }

        decimal Price { get; }

        DateTime CreateDate { get; }

        string Summary { get; }
    }
}
