using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public interface IContact : IUnique,IPersistence,IFulError,IFulSuccess
    {
        string ClientID { get; set; }

        string Name { get; set; }

        JobType JobTypes { get; set; }

        string CompanyID { get; set; }

        string Position { get; set; }

        string Email { get; set; }

        string Mobile { get; set; }

        string Tel { get; set; }

        string Detail { get; }

        Status Status { get; }

        DateTime CreateDate { get; }

        DateTime UpdateDate { get; }
    }
}
