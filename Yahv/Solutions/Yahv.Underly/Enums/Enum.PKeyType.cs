using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    [Obsolete("原则上：Layers.Data只能增加在逻辑层中，Yahv.Underly一开始的期望是只放一些通用的定义与持久层无关")]
    public enum PKeyType
    {
        /// <summary>
        /// 客户订单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("", PKeySigner.Mode.Date, 3)]
        Order = 10000, //TODO:客户编号+年月日+3位

        /// <summary>
        /// 客户订单项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("XDTOrderItem", PKeySigner.Mode.Date, 6)]
        OrderItem = 10001,

        /// <summary>
        /// 客户订单附件
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("OrderFile", PKeySigner.Mode.Date, 4)]
        OrderFile = 10002,

        /// <summary>
        /// 客户订单项（进项）
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Ipt", PKeySigner.Mode.Date, 8)]
        Input = 10003,

        /// <summary>
        /// 客户订单项（销项）
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Opt", PKeySigner.Mode.Date, 8)]
        Output = 10004,

        /// <summary>
        /// 运单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Waybill", PKeySigner.Mode.Date, 4)]
        Waybill = 20000,

        /// <summary>
        /// 运单费用
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("WayCost", PKeySigner.Mode.Date, 4)]
        WayCost = 20001,

        /// <summary>
        /// 入库单号
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("T", PKeySigner.Mode.Date, 4)]
        TempEnterCode = 20002,

        /// <summary>
        /// 清关费用记录
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Customs", PKeySigner.Mode.Date, 4)]
        CustomsRecords = 30000,

        /// <summary>
        /// 文档
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Docs", PKeySigner.Mode.Date, 4)]
        vDocuments = 10005,

        /// <summary>
        /// 分类
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Cxdt", PKeySigner.Mode.Normal, 4)]
        vCatalogs = 10006,

        #region 代仓储

        /// <summary>
        /// 代仓储客户端通知
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("wsNtc", PKeySigner.Mode.Date, 4)]
        wsNotice = 10010,

        /// <summary>
        /// 代仓储开票
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("IVNT", PKeySigner.Mode.Date, 4)]
        invoiceNotice = 10011,

        /// <summary>
        /// 代仓储开票通知项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("IVNTItem", PKeySigner.Mode.Date, 4)]
        invoiceNoticeItem = 10012,


        /// <summary>
        /// DCXReceipt 开出发票（全额发票、服务费发票）
        /// </summary>
        [Repository(typeof(PvWsOrderReponsitory))]
        [PKey("DCCReceipt", PKeySigner.Mode.Date, 10)]
        DCCReceipt = 21100,

        /// <summary>
        /// XDTRecFund 根据款项确认明细，冲预收，做应收
        /// </summary>
        [Repository(typeof(PvWsOrderReponsitory))]
        [PKey("DCCRecFund", PKeySigner.Mode.Date, 10)]
        DCCRecFund = 21200,

        #endregion

        #region LsOrder
        /// <summary>
        /// 租赁订单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("LsOrder", PKeySigner.Mode.Date, 4)]
        LsOrder = 70000,

        /// <summary>
        /// 租赁订单项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("LsOrderItem", PKeySigner.Mode.Date, 4)]
        LsOrderItem = 70001,

        #endregion

        #region Erm
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

        ///// <summary>
        ///// 银行卡信息
        ///// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("Bank", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Staff')+1 ,LEN([ID]) - LEN('Staff')))) FROM [PvbErm].[dbo].[BankCards]where [ID] like 'Staff%'")]
        //BankCard = 50002,

        ///// <summary>
        ///// 劳资信息
        ///// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("Labour", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Staff')+1 ,LEN([ID]) - LEN('Staff')))) FROM [PvbErm].[dbo].[BankCards]where [ID] like 'Staff%'")]
        //Labour = 50003,

        /// <summary>
        /// 工资
        /// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("PayItem", PKeySigner.Mode.Normal, 5)]
        //PayItem = 50004,

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
        [PKey("Staff", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Staff')+1 ,LEN([ID]) - LEN('Staff')))) FROM [PvbErm].[dbo].[BankCards] where [ID] like 'Staff%'")]
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
        [PKey("AttendLog", PKeySigner.Mode.Date, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('AttendLog{datetime}')+1 ,LEN([ID]) - LEN('AttendLog{datetime}')))) FROM [PvbErm].[dbo].[Logs_Attend] where [ID] like 'AttendLog{datetime}%'")]
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
        [PKey("FamilyMember", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('FamilyMember')+1 ,LEN([ID]) - LEN('FamilyMember')))) FROM [PvbErm].[dbo].[PersonalFamilyMembers] where [ID] like 'FamilyMember%'")]
        FamilyMember = 50019,

        /// <summary>
        /// 账号密码历史表
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("PastsPwd", PKeySigner.Mode.Normal, 5, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('PastsPwd')+1 ,LEN([ID]) - LEN('PastsPwd')))) FROM [PvbErm].[dbo].[Pasts_AdminPassword] where [ID] like 'PastsPwd%'")]
        PastsAdminPassword = 50020,
        #endregion

        #region Pays
        /// <summary>
        /// 流水账
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Flow", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Flow{datetime}')+1 ,LEN([ID]) - LEN('Flow{datetime}')))) FROM [PvbCrm].[dbo].[FlowAccounts] where [ID] like 'Flow{datetime}%'")]
        FlowAccount = 60001,

        /// <summary>
        /// 账期日志
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("LogsDebt", PKeySigner.Mode.Normal, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('LogsDebt')+1 ,LEN([ID]) - LEN('LogsDebt')))) FROM [PvbCrm].[dbo].[Logs_DebtTerms] where [ID] like 'LogsDebt%'")]
        LogsDebtTerms = 60002,

        /// <summary>
        /// 应收款项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Receb", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Receb{datetime}')+1 ,LEN([ID]) - LEN('Receb{datetime}')))) FROM [PvbCrm].[dbo].[Receivables] where [ID] like 'Receb{datetime}%'")]
        Receivables = 60003,

        /// <summary>
        /// 实收款项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Reced", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Reced{datetime}')+1 ,LEN([ID]) - LEN('Reced{datetime}')))) FROM [PvbCrm].[dbo].[Receiveds] where [ID] like 'Reced{datetime}%'")]
        Receiveds = 60004,

        /// <summary>
        /// 应付款项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Paybl", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Paybl{datetime}')+1 ,LEN([ID]) - LEN('Paybl{datetime}')))) FROM [PvbCrm].[dbo].[Payables] where [ID] like 'Paybl{datetime}%'")]
        Payables = 60005,

        /// <summary>
        /// 实付款项
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Paymt", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Paymt{datetime}')+1 ,LEN([ID]) - LEN('Paymt{datetime}')))) FROM [PvbCrm].[dbo].[Payments] where [ID] like 'Paymt{datetime}%'")]
        Payments = 60006,

        /// <summary>
        /// 财务通知单
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Vou", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Vou{datetime}')+1 ,LEN([ID]) - LEN('Vou{datetime}')))) FROM [PvbCrm].[dbo].[Vouchers] where [ID] like 'Vou{datetime}%'")]
        Vouchers = 60007,

        /// <summary>
        /// 财务通知记录
        /// </summary>
        //[Repository(typeof(PvCenterReponsitory))]
        //[PKey("VouRec", PKeySigner.Mode.Date, 4)]
        //VoucherRecords = 60008,

        /// <summary>
        /// 优惠券
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Coupon", PKeySigner.Mode.Date, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Coupon{datetime}')+1 ,LEN([ID]) - LEN('Coupon{datetime}')))) FROM [PvbCrm].[dbo].[Coupons] where [ID] like 'Coupon{datetime}%'")]
        Coupon = 60009,

        /// <summary>
        /// 优惠券流水
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("FlowCoupon", PKeySigner.Mode.Date, 6, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('FlowCoupon{datetime}')+1 ,LEN([ID]) - LEN('FlowCoupon{datetime}')))) FROM [PvbCrm].[dbo].[FlowCoupons] where [ID] like 'FlowCoupon{datetime}%'")]
        FlowCoupon = 60010,

        /// <summary>
        /// 科目
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Subjects", PKeySigner.Mode.Normal, 4, @"SELECT CONVERT (int , MAX(substring ([ID],LEN('Subjects')+1 ,LEN([ID]) - LEN('Subjects')))) FROM [PvbCrm].[dbo].[Subjects] where [ID] like 'Subjects%'")]
        Subjects = 60012,
        #endregion

        #region Application

        /// <summary>
        /// 收付款申请
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("Apply", PKeySigner.Mode.Date, 4)]
        Application = 80000,

        #endregion

        #region ScCustoms
        /// <summary>
        /// 付汇申请文件
        /// </summary>
        [Repository(typeof(ScCustomReponsitory))]
        [PKey("PEAFile", PKeySigner.Mode.Date, 4)]
        PayExchangeApplyFile = 90002,
        #endregion

        #region Crm  PvbCrm
        /// <summary>
        /// 付款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Payer", PKeySigner.Mode.Normal, 4)]
        Payer = 11000,

        /// <summary>
        /// 收款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Payee", PKeySigner.Mode.Normal, 4)]
        Payee = 11001,

        /// <summary>
        /// 企业
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Ep", PKeySigner.Mode.Normal, 5)]
        Enterprise = 11002,

        /// <summary>
        /// 私有付款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nPayer", PKeySigner.Mode.Normal, 5)]
        nPayer = 11003,
        /// <summary>
        /// 私有收款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nPayee", PKeySigner.Mode.Normal, 5)]
        nPayee = 11004,
        /// <summary>
        /// 私有供应商
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nSupplier", PKeySigner.Mode.Normal, 5)]
        nSupplier = 11005,
        /// <summary>
        /// 私有交货地址
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nConsignor", PKeySigner.Mode.Normal, 5)]
        nConsignor = 11006,
        /// <summary>
        /// 私有联系人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nContact", PKeySigner.Mode.Normal, 5)]
        nContact = 11007,
        #endregion

        #region  PvData

        [Repository(typeof(PvDataReponsitory))]
        [PKey("WK", PKeySigner.Mode.Date, 5)]
        Logs_WayjdByKeyword = 50003,

        [Repository(typeof(PvDataReponsitory))]
        [PKey("WN", PKeySigner.Mode.Date, 5)]
        Logs_WayjdByName = 50002,
        #endregion

        /// <summary>
        /// 日志
        /// </summary>
        [Repository(typeof(PvWmsRepository))]
        [PKey("LOP", PKeySigner.Mode.Date, 6)]
        Logs_Operator,
    }
}
