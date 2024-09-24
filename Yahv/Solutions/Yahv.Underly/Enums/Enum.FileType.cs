using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 文件类型
    /// </summary>
    /// <remarks>
    /// 如果华芯通已经存在使用华芯通的数值
    /// 再增加的时候 从这里9000开始
    /// </remarks>
    public enum FileType : int
    {
        #region 与华芯通文件类型值保持一致，勿改
        [Description("对账单")]
        OrderBill = 1,

        /// <summary>
        /// 付汇委托书
        /// </summary>
        [Description("付汇委托书")]
        PayExchange = 2,

        [Description("代理报关委托书")]
        AgentTrustInstrument = 3,

        [Description("合同发票(PI)")]
        Invoice = 5,

        /// <summary>
        /// 营业执照
        /// </summary>
        [Description("营业执照")]
        BusinessLicense = 7,
        /// <summary>
        /// 授权委托书
        /// </summary>
        [Description("授权委托书")]
        PowerOfAttorney = 8,

        [Description("提货文件")]
        Delivery = 10,

        /// <summary>
        /// 付汇PI文件
        /// </summary>
        [Description("付汇PI文件")]
        PIFiles = 11,

        [Description("订单费用附件")]
        OrderFeeFile = 15,

        /// <summary>
        /// 服务协议
        /// </summary>
        [Description("服务协议")]
        ServiceAgreement = 22,

        [Description("标签文件")]
        Label = 24,

        [Description("随货文件")]
        FollowGoods = 25,

        [Description("代收货款委托书")]
        ReceiveEntrust = 26,

        [Description("代付货款委托书")]
        PaymentEntrust = 27,

        [Description("租赁合同")]
        Contract = 28,

        [Description("报关委托书")]
        Declaration = 29,

        /// <summary>
        /// 装箱单，指导库房装箱
        /// </summary>
        [Description("装箱单")]
        Packing = 30,

        /// <summary>
        /// 代收代发代管协议
        /// </summary>
        [Description("代收代发代管协议")]
        WsAgreement = 40,

        [Description("企业Logo")]
        EnterpriseLogo = 41,

        /// <summary>
        /// 用户头像
        /// </summary>
        [Description("用户头像")]
        Avatar = 42,

        /// <summary>
        /// 华芯通境内销售合同（增值税开票时，税务要求）ryan 20200918
        /// </summary>
        [Description("销售合同")]
        SalesContract = 50,
        /// <summary>
        /// 登记证
        /// </summary>
        [Description("登记证")]
        HKBusinessLicense = 60,
        /// <summary>
        /// 仓储协议
        /// </summary>
        [Description("仓储协议")]
        StorageAgreement = 70,
        #endregion


        // 1000 - 1100 给融合发票使用

        #region 给融合发票使用

        [Description("增值税专用发票")]
        VATInvoice = 1000,

        [Description("增值税普通发票")]
        OrdinaryInvoice = 1002,


        [Description("海关增值税普通发票")]
        CustomsVATInvoice = 1003,

        [Description("海关关税发票")]
        CustomsTariffInvoice = 1004,

        [Description("海关消费税发票")]
        CustomsExciseTaxInvoice = 1005,

        #endregion

        #region Erm文件类型 2000 - 3000

        [Description("身份证复印件")]
        IDCardCopy = 2000,

        [Description("入职照片")]
        InductionImage = 2001,

        [Description("入职登记表")]
        InductionRegistrationForm = 2002,

        [Description("离职证明")]
        LeavingCertificate = 2003,

        [Description("毕业证复印件")]
        DiplomaCopy = 2004,

        [Description("户口首页复印件")]
        ResidenceBookFirstPage = 2005,

        [Description("本人户口复印件")]
        ResidenceBookSelfPage = 2006,

        [Description("劳动合同")]
        LaborContract = 2007,

        [Description("体检证明")]
        HealthCertificate = 2008,

        [Description("应聘人员登记表")]
        EmploymentForm = 2009,

        [Description("面试情况评估表")]
        InterviewEvaluationForm = 2010,

        [Description("工资银行卡复印件")]
        BankcardCopy = 2011,

        [Description("员工背景调查报告")]
        BackgroundInvestigation = 2012,

        [Description("转正申请附件")]
        TurnNormal = 2013,

        [Description("公司人员花名册")]
        Roster = 2014,

        [Description("加班申请附件")]
        OvertimeApplication = 2100,

        [Description("请假申请附件")]
        OfftimeApplication = 2150,

        [Description("补签申请附件")]
        ResignApplication = 2180,

        [Description("印章借用申请附件")]
        SealApplication = 2181,

        [Description("工牌补办申请附件")]
        WorkCardApplication = 2182,

        [Description("培训申请附件")]
        InternalTrainingApplication = 2183,

        [Description("外训申请附件")]
        ExternalTrainingApplication = 2184,

        [Description("单证档案借阅申请附件")]
        ArchiveBorrowApplication = 2185,

        [Description("单证档案外借申请附件")]
        ArchiveLendingApplication = 2186,

        [Description("单证档案销毁申请附件")]
        ArchiveDestroyApplication = 2187,

        [Description("印章刻制申请附件")]
        SealEngraveApplication = 2188,

        [Description("员工奖惩申请附件")]
        RewardAndPunishApplication = 2189,

        /// <summary>
        /// 离职协议
        /// </summary>
        [Description("离职协议")]
        ResignationAgreement = 2200,

        /// <summary>
        /// 物资移交清单
        /// </summary>
        //[Description("物资移交清单")]
        //ResignationMaterial = 2201,



        /// <summary>
        /// 离职申请表
        /// </summary>
        [Description("离职申请表")]
        ResignationApplication = 2202,

        /// <summary>
        /// 离职交接表
        /// </summary>
        [Description("离职交接表")]
        ResignationHandover = 2203,

        [Description("其它")]
        ErmOrhers = 2999,

        #endregion

        #region 财务模块文件类型 3000-4000
        [Description("收款凭证")]
        ReceiptVoucher = 3000,

        [Description("付款凭证")]
        PaymentVoucher = 3010,
        #endregion

        #region 询报价备案附件 4001-5000
        /// <summary>
        /// 合同
        /// </summary>
        [Description("合同")]
        RfqContract = 4001,

        /// <summary>
        /// 对账单
        /// </summary>
        [Description("对账单")]
        RfqBill = 4002,

        /// <summary>
        /// 框架协议
        /// </summary>
        [Description("框架协议")]
        RfqFa = 4003,

        /// <summary>
        /// 承诺书
        /// </summary>
        [Description("承诺书")]
        RfqLoc = 4004,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        RfqOther = 4500,
        #endregion

        [Description("库存图片")]
        StoragesPic = 8000,

        [Description("发货文件")]
        DeliverGoods = 8010,

        [Description("送货确认文件")]
        SendGoods = 8020,

        //如果华芯通已经存在使用华芯通的数值
        //再增加的时候 从这里 9000 开始

        [Description("测试文件")]
        Test = 9000,

        [Description("产品导入文件")]
        ProductImportFile = 100001,

        [Description("客户账单")]
        ClientBillFile = 100002,

        #region 大赢家备案文件附件 101000-102000
        /// <summary>
        /// 合同
        /// </summary>
        [Description("合同")]
        DyjContract = 101001,

        /// <summary>
        /// 对账单
        /// </summary>
        [Description("对账单")]
        DyjBill = 101002,

        /// <summary>
        /// 框架协议
        /// </summary>
        [Description("框架协议")]
        DyjAgreement = 101003,

        /// <summary>
        /// 承诺书
        /// </summary>
        [Description("承诺书")]
        DyjCommitment = 101004,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        DyjOthers = 101005,
        #endregion

        [Description("SF快递单")]
        SFImages = 101006,
    }

}
