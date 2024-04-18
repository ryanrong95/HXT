using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class SalesView : AdminProjectViewBase
    {
        public SalesView(IGenericAdmin admin, JobType jobType = JobType.Sales) : base(jobType)
        {

        }
    }
}
