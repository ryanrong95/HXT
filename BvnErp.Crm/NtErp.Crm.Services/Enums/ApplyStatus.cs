using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ApplyStatus
    {
        [Description("待审批")]
        Audting = 10,

        [Description("审批通过")]
        Approval = 20,

        [Description("审批不通过")]
        NotApproval = 30 
    }
}
