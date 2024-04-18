using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 付款类型中经营类枚举
    /// 注意该枚举值 >20000, 要被 FeeTypeEnum 包含
    /// </summary>
    public enum CostTypeEnum
    {
        退款 = 20001,

        还款 = 20002,

        资金调拨 = 20003,

        资金往来 = 20004,

        借款 = 20005,

        付汇 = 20006,

        费用 = 20007,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FeeTypeEnum
    {
        无 = 0,

        //税费 = 10001,

        //工资 = 10002,

        //社保费 = 10003,

        //公积金 = 10004,

        //租金 = 10005,

        //物业管理费 = 10006,

        //水电费 = 10007,

        //电话费 = 10008,

        //宽带网络费 = 10009,

        //快递费 = 10010,

        //运费 = 10011,

        //停车费 = 10012,

        //加油费 = 10013,

        //路桥费 = 10014,

        //汽车保险费 = 10015,

        //汽车维修费 = 10016,

        //审计费 = 10017,

        //咨询费 = 10018,

        //业务招待费 = 10019,

        //职工福利经费 = 10020,

        //培训费 = 10021,

        //广告费 = 10022,

        //劳务费 = 10023,

        //印刷费 = 10024,

        //工会经费 = 10025,

        //残疾人就业保障金 = 10026,

        //差旅费 = 10027,

        //差旅补助 = 10028,

        //办公用品采购 = 10029,

        //维修费 = 10030,

        邮电费 = 10001,

        租赁费 = 10002,

        办公设备 = 10003,

        工资 = 10004,
        
        差旅费 = 10005,

        运杂费 = 10006,

        广告费 = 10007,

        会员费 = 10008,

        福利费 = 10009,

        水电费 = 10010,

        低值易耗品 = 10011,

        汽车支出 = 10012,

        劳动保险费 = 10013,

        利息 = 10014,

        银行费用 = 10015,

        汇换损益 = 10016,

        税金及附加 = 10017,

        域名费 = 10018,

        物业费 = 10019,

        业务活动费用 = 10020,

        残保金 = 10021,



        其它 = 19990,



        //注意 >20000 的枚举要包含 CostTypeEnum

        退款 = 20001,

        还款 = 20002,

        资金调拨 = 20003,

        资金往来 = 20004,

        借款 = 20005,

        付汇 = 20006,

        费用 = 20007,
    }

    public enum CostStatusEnum
    {
        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        Cancel = 1,

        /// <summary>
        /// 未提交
        /// </summary>
        [Description("未提交")]
        UnSubmit = 2,

        /// <summary>
        /// 待财务负责人审批
        /// </summary>
        [Description("待财务负责人审批")]
        FinanceStaffUnApprove = 3,

        /// <summary>
        /// 待经理审批
        /// </summary>
        [Description("待经理审批")]
        ManagerUnApprove = 4,

        /// <summary>
        /// 待付款
        /// </summary>
        [Description("待付款")]
        UnPay = 5,

        /// <summary>
        /// 付款成功
        /// </summary>
        [Description("付款成功")]
        PaySuccess = 6
    }

    public enum CostApplyFileTypeEnum
    {
        /// <summary>
        /// 单据
        /// </summary>
        [Description("单据")]
        Inovice = 1,
    }

    /// <summary>
    /// 区分费用申请类型（个人申请费用、银行自动扣款）
    /// </summary>
    public enum MoneyTypeEnum
    {
        /// <summary>
        /// 个人申请费用
        /// </summary>
        [Description("申请费用")]
        IndividualApply = 1,

        /// <summary>
        /// 银行自动扣款
        /// </summary>
        [Description("银行自动扣款")]
        BankAutoApply = 2,
    }

    public enum CashTypeEnum
    {
        /// <summary>
        /// 普通非现金
        /// </summary>
        [Description("非现金")]
        Common = 1,

        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 2,
    }

    /// <summary>
    /// 是否收到纸质票据
    /// </summary>
    public enum CheckPaperNotesEnum
    {
        /// <summary>
        /// 有纸质票据
        /// </summary>
        [Description("是")]
        PaperNotes = 1,
        /// <summary>
        /// 无纸质票据
        /// </summary>
        [Description("否")]
        UnPaperNotes = 0,
    }
}
