using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 管理员
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Admin", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Admin')+1 ,LEN([ID]) - LEN('Admin')))) FROM [PvbErm].[dbo].[Admins] where [ID] like 'Admin%'")]
        Admin = 50000,

        /// <summary>
        /// 角色
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Role", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Role')+1 ,LEN([ID]) - LEN('Role')))) FROM [PvbErm].[dbo].[Roles] where [ID] like 'Role%'")]
        Role = 50001,

        /// <summary>
        /// 工资项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("WageItem", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('WageItem')+1 ,LEN([ID]) - LEN('WageItem')))) FROM [PvbErm].[dbo].[WageItems] where [ID] like 'WageItem%'")]
        WageItem = 50005,

        /// <summary>
        /// 员工
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Staff", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Staff')+1 ,LEN([ID]) - LEN('Staff')))) FROM [PvbErm].[dbo].[Staffs] where [ID] like 'Staff%'")]
        Staff = 50006,

        /// <summary>
        /// 工资默认值历史记录
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("PWageItem", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('PWageItem')+1 ,LEN([ID]) - LEN('PWageItem')))) FROM [PvbErm].[dbo].[Pasts_MapsWageItem] where [ID] like 'PWageItem%'")]
        Pasts_MapsWageItem = 50007,

        /// <summary>
        /// 考勤日志
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("AttendLog", PKeySigner.Mode.Date, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('AttendLog')+1 ,LEN([ID]) - LEN('AttendLog')))) FROM [PvbErm].[dbo].[Logs_Attend] where [ID] like 'AttendLog%'")]
        AttendLog = 50009,

        /// <summary>
        /// 日程安排表
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Sched", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Sched')+1 ,LEN([ID]) - LEN('Sched')))) FROM [PvbErm].[dbo].[Schedules] where [ID] like 'Sched%'")]
        Sched = 50010,

        /// <summary>
        /// 班别
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Scheing", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Scheing')+1 ,LEN([ID]) - LEN('Scheing')))) FROM [PvbErm].[dbo].[Schedulings] where [ID] like 'Scheing%'")]
        Scheing = 50011,

        /// <summary>
        /// 员工假期
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Vacation", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Vacation')+1 ,LEN([ID]) - LEN('Vacation')))) FROM [PvbErm].[dbo].[Vacations] where [ID] like 'Vacation%'")]
        Vacation = 50012,

        /// <summary>
        /// 员工申请
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("ErmApply", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('ErmApply')+1 ,LEN([ID]) - LEN('ErmApply')))) FROM [PvbErm].[dbo].[Applications] where [ID] like 'ErmApply%'")]
        Erm_Application = 50013,

        /// <summary>
        /// 申请审批步骤
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("ApplyVS", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('ApplyVS')+1 ,LEN([ID]) - LEN('ApplyVS')))) FROM [PvbErm].[dbo].[ApplyVoteSteps] where [ID] like 'ApplyVS%'")]
        ApplyVoteStep = 50014,

        /// <summary>
        /// 申请审批步骤
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("ApplyVSLog", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('ApplyVSLog')+1 ,LEN([ID]) - LEN('ApplyVSLog')))) FROM [PvbErm].[dbo].[Logs_ApplyVoteSteps] where [ID] like 'ApplyVSLog%'")]
        ApplyVoteStepLog = 50015,

        ///// <summary>
        ///// 审批流
        ///// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("VoteFlow", PKeySigner.Mode.Normal, 5)]
        //VoteFlow = 50016,

        ///// <summary>
        ///// 审批步骤
        ///// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("VoteStep", PKeySigner.Mode.Normal, 5)]
        //VoteStep = 50017,

        /// <summary>
        /// 员工工作经历
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("WorkExp", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('WorkExp')+1 ,LEN([ID]) - LEN('WorkExp')))) FROM [PvbErm].[dbo].[PersonalWorkExperiences] where [ID] like 'WorkExp%'")]
        WorkExp = 50018,

        /// <summary>
        /// 员工家庭成员
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("FmlyMbr", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('FmlyMbr')+1 ,LEN([ID]) - LEN('FmlyMbr')))) FROM [PvbErm].[dbo].[PersonalFamilyMembers] where [ID] like 'FmlyMbr%'")]
        FamilyMember = 50019,

        /// <summary>
        /// 账号密码历史表
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("PastsPwd", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('PastsPwd')+1 ,LEN([ID]) - LEN('PastsPwd')))) FROM [PvbErm].[dbo].[Pasts_AdminPassword] where [ID] like 'PastsPwd%'")]
        PastsAdminPassword = 50020,

        /// <summary>
        /// 印章证照表
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("SealCtf", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('SealCtf')+1 ,LEN([ID]) - LEN('SealCtf')))) FROM [PvbErm].[dbo].[SealCertificates] where [ID] like 'SealCtf%'")]
        SealCtf = 50021,
    }
}