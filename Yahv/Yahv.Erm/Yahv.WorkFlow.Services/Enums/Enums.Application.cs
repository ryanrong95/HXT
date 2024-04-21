using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.WorkFlow.Services
{
    /// <summary>
    /// 申请状态
    /// </summary>
    public enum ApplicationStatus
    {
        [Description("废弃")]
        Abandoned = 1,

        [Description("草稿")]
        Draft = 2,

        [Description("驳回")]
        Rejected = 3,

        [Description("审批中")]
        UnderApproval = 4,

        [Description("已完成")]
        Completed = 5,
    }

    /// <summary>
    /// 申请审批状态
    /// </summary>
    public enum ApplyVoteStepStatus
    {
        [Description("赞同")]
        Allow = 1,

        [Description("否决")]
        Veto = 0,

        [Description("待审批")]
        ApprovalPending = 2,
    }
}
