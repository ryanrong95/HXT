using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public interface IReportItem 
    {
        string FollowUpMethod {get;}

         DateTime FollowUpDate { get; }

         DateTime NextFollowUpDate { get; }

        string FollowUpAdminID { get; }

        string FollowUpAdminName { get; }

        string FollowUpContent { get; }

    }
}
