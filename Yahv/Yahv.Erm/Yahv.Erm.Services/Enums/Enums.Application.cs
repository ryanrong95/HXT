using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
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

        [Description("招聘申请")]
        Recruit = 6,

        [Description("印章借用申请")]
        SealBorrow = 7,

        [Description("工牌补办申请")]
        WorkCard = 8,

        [Description("培训申请")]
        InternalTraining = 9,

        [Description("外训申请")]
        ExternalTraining = 10,

        [Description("单证档案借阅申请")]
        ArchiveBorrow = 11,

        [Description("单证档案外借申请")]
        ArchiveLending = 12,

        [Description("单证档案销毁申请")]
        ArchiveDestroy = 13,

        [Description("印章刻制申请")]
        SealEngrave = 14,

        [Description("员工奖惩申请")]
        RewardAndPunish = 15,
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

        [Description("已完成")]
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

    /// <summary>
    /// 招聘出差需求
    /// </summary>
    public enum BussnessTripRequirement
    {
        [Description("不需要")]
        NotNeed = 1,

        [Description("偶尔")]
        NotOften = 2,

        [Description("经常")]
        Often = 3,
    }

    /// <summary>
    /// 招聘需求等级
    /// </summary>
    public enum EmergentRequirement
    {
        [Description("不紧急")]
        NotEmergent = 1,

        [Description("一般紧急")]
        GeneralEmergent = 2,

        [Description("紧急")]
        Emergent = 3,
    }

    /// <summary>
    /// 招聘性别需求
    /// </summary>
    public enum GenderRequirement
    {
        [Description("男")]
        Male = 1,

        [Description("女")]
        Female = 2,

        [Description("不限")]
        MaleOrFemale = 3,
    }

    /// <summary>
    /// 招聘学历要求
    /// </summary>
    public enum EducationRequirement
    {
        [Description("大专及以上")]
        CollegeDegree = 1,

        [Description("本科及以上")]
        BachelorDegree = 2,

        [Description("硕士及以上")]
        MasterDegree = 3,

        [Description("其它")]
        OtherDegree = 4,
    }

    /// <summary>
    /// 印章证照类型
    /// </summary>
    public enum SealBorrowType
    {
        [Description("使用")]
        使用 = 1,

        [Description("借用")]
        借用 = 2,
    }

    /// <summary>
    /// 印章形状
    /// </summary>
    public enum SealShape
    {
        [Description("椭圆")]
        椭圆 = 1,

        [Description("正圆")]
        正圆 = 2,

        [Description("方形")]
        方形 = 3,

        [Description("矩形")]
        矩形 = 4,

        [Description("其它")]
        其它 = 5,
    }

    /// <summary>
    /// 培训方式
    /// </summary>
    public enum TrainingMethod
    {
        [Description("授课")]
        授课 = 1,

        [Description("座谈")]
        座谈 = 2,

        [Description("现场操作指导")]
        现场操作指导 = 3,

        [Description("媒体讲解")]
        媒体讲解 = 4,

        [Description("资料学习")]
        资料学习 = 5,

        [Description("其它方式")]
        其它方式 = 99,
    }

    /// <summary>
    /// 奖惩类别
    /// </summary>
    public enum RewardOrPunish
    {
        [Description("奖励")]
        奖励 = 1,

        [Description("惩罚")]
        惩罚 = 2,
    }

    /// <summary>
    /// 奖励方式
    /// </summary>
    public enum RewardType
    {
        [Description("嘉奖")]
        嘉奖 = 1,

        [Description("记功")]
        记功 = 2,

        [Description("记大功")]
        记大功 = 3,

        [Description("晋级")]
        晋级 = 4,

        [Description("特殊奖励")]
        特殊奖励 = 5,
    }

    /// <summary>
    /// 惩罚方式
    /// </summary>
    public enum PunishType
    {
        [Description("警告")]
        警告 = 1,

        [Description("记过")]
        记过 = 2,

        [Description("辞退")]
        辞退 = 3,

        [Description("开除")]
        开除 = 4,
    }
}
