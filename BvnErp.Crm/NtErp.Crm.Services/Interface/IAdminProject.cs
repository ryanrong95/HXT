using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;

namespace NtErp.Crm.Services.Models
{
    public interface IAdminProject : IPersistence,IFulError,IFulSuccess
    {
        string AdminID { get; set; }

        string CompanyID { get; set; }

        JobType JobType { get; set; }

        DateTime CreateDate { get; set; }

        DateTime UpdateDate { get; set; }

        string Summary { get; set; }
    }
}
