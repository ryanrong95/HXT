using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 申请的类型
    /// </summary>
    public enum ApplicationType
    {
        [Description("入职申请")]
        Entry = 1,

        [Description("离职申请")]
        Leave = 2,

        [Description("加班申请")]
        Overtime = 3,

        [Description("请假申请")]
        Offtime = 4,

        [Description("补签申请")]
        ReSign = 5,
    }

    /// <summary>
    /// 申请的状态
    /// </summary>
    public enum ApplicationStatus
    {
        [Description("草稿")]
        Draft = 1,

        [Description("审批中")]
        UnderApproval = 2,

        [Description("完成")]
        Complete = 3,

        [Description("驳回")]
        Reject = 4,

        [Description("废弃")]
        Delete = 400,
    }

    /// <summary>
    /// 审批的状态
    /// </summary>
    public enum ApprovalStatus
    {
        [Description("等待")]
        Waiting = 100,

        [Description("同意")]
        Agree = 200,

        [Description("驳回")]
        Reject = 400,
    }

    /// <summary>
    /// 请假时长类型
    /// </summary>
    public enum DateLengthType
    {
        [Description("整天")]
        AllDay = 0,

        [Description("上午")]
        Morning = 1,

        [Description("下午")]
        Afternoon = 2,
    }

    /// <summary>
    /// 出差原因类型
    /// </summary>
    public enum BusinessTripReason
    {
        [Description("拜访客户")]
        VisitClient = 1,

        [Description("拜访供应商")]
        VisitSupplier = 2,

        [Description("引进人员")]
        IntroductPersonal = 3,

        [Description("其它商务合作")]
        Others = 99,
    }

    /// <summary>
    /// 是否借款
    /// </summary>
    public enum LoanOrNot
    {
        [Description("否")]
        Not = 0,

        [Description("是")]
        Yes = 1,
    }
}
