using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 员工状态
    /// </summary>
    public enum StaffStatus
    {
        [Description("未面试")]
        UnApplied = 98,

        [Description("面试未通过")]
        InterviewFail = 99,

        [Description("面试已通过")]
        InterviewPass = 100,

        [Description("入职申请中")]
        Applied = 101,

        [Description("试用期")]
        Period = 102,

        [Description("在职")]
        Normal = 200,

        [Description("离职")]
        Departure = 300,

        [Description("废弃")]
        Delete = 400,

        [Description("注销")]
        Cancel = 500,
    }

    /// <summary>
    /// 员工入职审批步骤
    /// </summary>
    public enum StaffApprovalStep
    {
        [Description("面试")]
        Interview = 1,

        [Description("背景调查")]
        BackgroundInvestigate = 2,

        [Description("经理审批")]
        Manager = 3,

        [Description("入职通知")]
        Notice = 4,

        [Description("入职报到")]
        Entry = 5,
    }

    /// <summary>
    /// 通用审批状态
    /// </summary>
    public enum StaffApprovalStatus
    {
        [Description("等待")]
        Waiting = 100,

        [Description("已申请")]
        Applied = 101,

        [Description("已通过")]
        Pass = 200,

        [Description("未通过")]
        Fail = 400,
    }

    /// <summary>
    /// 报到通知状态
    /// </summary>
    public enum StaffNoticeReportStatus
    {
        [Description("未通知")]
        UnNotified = 100,

        [Description("已通知")]
        Notified = 200,
    }

    /// <summary>
    /// 报到入职状态
    /// </summary>
    public enum StaffEntryReportStatus
    {
        [Description("等待报到")]
        WaitingReport = 100,

        [Description("已申请")]
        Applied = 101,

        [Description("已入职")]
        Entry = 200,

        [Description("未报到")]
        UnReport = 400,
    }
}