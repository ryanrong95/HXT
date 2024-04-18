using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    public class FaeView : AdminProjectViewBase
    {
        public FaeView(IGenericAdmin admin, JobType jobType = JobType.FAE) : base(jobType)
        {
        }
    }
}
