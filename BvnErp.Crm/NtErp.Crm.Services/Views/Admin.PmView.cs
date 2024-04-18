using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class PmView : AdminProjectViewBase
    {
        public PmView(IGenericAdmin admin, JobType jobType = JobType.PME) : base(jobType)
        {

        }
    }
}
