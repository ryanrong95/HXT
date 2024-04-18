using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Enums
{
    public enum ActionStatus
    {
        [Description("待审批")]
        Auditing = 100,
        [Description("正常")]
        Normal = 200,
        [Description("删除")]
        Delete = 400,
        [Description("审批通过")]
        Complete = 500,
        [Description("审批不通过")]
        Reject = 600
    }

    public enum Status
    {
        [Description("暂存")]
        Temporary = 100,

        [Description("正常")]
        Normal = 200,

        [Description("删除")]
        Delete = 400
    }
}
