using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.Finance.Services
{
    public enum PKeyType
    {
        /// <summary>
        /// 收付款类型
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("AccCatType", PKeySigner.Mode.Normal, 4)]
        AccCatType,

        /// <summary>
        /// 金库
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("GoldStore", PKeySigner.Mode.Normal, 4)]
        GoldStore,

        /// <summary>
        /// 银行
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("Bank", PKeySigner.Mode.Normal, 4)]
        Bank,

        /// <summary>
        /// 银行风险地区
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("BankRiskArea", PKeySigner.Mode.Normal, 4)]
        BankRiskArea,

        /// <summary>
        /// 企业
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("Enterprise", PKeySigner.Mode.Normal, 5)]
        Enterprise,

        /// <summary>
        /// 账户
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("Account", PKeySigner.Mode.Normal, 5)]
        Account,

        /// <summary>
        /// 文件描述
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("FilesDesc", PKeySigner.Mode.Normal, 5)]
        FilesDesc,

        /// <summary>
        /// 文件属性
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("FilesMap", PKeySigner.Mode.Normal, 5)]
        FilesMap,

        /// <summary>
        /// 费用明细
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("CostCat", PKeySigner.Mode.Normal, 5)]
        CostCat,

        /// <summary>
        /// 收款左表
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("PayeeLeft", PKeySigner.Mode.Normal, 5)]
        PayeeLeft,

        /// <summary>
        /// 收款核销
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("PayeeRight", PKeySigner.Mode.Normal, 5)]
        PayeeRight,

        /// <summary>
        /// 付款申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("PayerApply", PKeySigner.Mode.Normal, 5)]
        PayerApply,

        /// <summary>
        /// 付款应付
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("PayerLeft", PKeySigner.Mode.Normal, 5)]
        PayerLeft,

        /// <summary>
        /// 付款实付
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("PayerRight", PKeySigner.Mode.Normal, 5)]
        PayerRight,

        /// <summary>
        /// 流水
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("FlowAcc", PKeySigner.Mode.Normal, 5)]
        FlowAcc,

        /// <summary>
        /// 审批日志
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("LogsApply", PKeySigner.Mode.Date, 5)]
        LogsApply,

        /// <summary>
        /// 资金申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("CostApply", PKeySigner.Mode.Normal, 5)]
        CostApply,

        /// <summary>
        /// 资金申请项
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("CostApplyItem", PKeySigner.Mode.Normal, 5)]
        CostApplyItem,

        /// <summary>
        /// 流水表日志
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("LogsFlow", PKeySigner.Mode.Date, 5)]
        LogsFlow,

        /// <summary>
        /// 资金调拨申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("SelfApply", PKeySigner.Mode.Normal, 5)]
        SelfApply,

        /// <summary>
        /// 资金调拨 应调
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("SelfLeft", PKeySigner.Mode.Normal, 5)]
        SelfLeft,

        /// <summary>
        /// 资金调拨 实调
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("SelfRight", PKeySigner.Mode.Normal, 5)]
        SelfRight,

        /// <summary>
        /// 费用申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("ChgApply", PKeySigner.Mode.Normal, 5)]
        ChargeApply,

        /// <summary>
        /// 费用申请项
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("ChgApplyItem", PKeySigner.Mode.Normal, 5)]
        ChargeApplyItem,

        /// <summary>
        /// 人员
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("Person", PKeySigner.Mode.Normal, 5)]
        Persons,

        /// <summary>
        /// 操作日志
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("LogsOp", PKeySigner.Mode.Date, 5)]
        LogsOprate,

        /// <summary>
        /// 税率
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("TaxRate", PKeySigner.Mode.Normal, 5)]
        TaxRates,

        /// <summary>
        /// 工资申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("SalaryApply", PKeySigner.Mode.Normal, 5)]
        SalaryApplies,

        /// <summary>
        /// 工资申请项
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("SalaryItems", PKeySigner.Mode.Normal, 5)]
        SalaryApplyItems,

        /// <summary>
        /// 认领表
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("AccWorks", PKeySigner.Mode.Normal, 5)]
        AccountWorks,

        /// <summary>
        /// 承兑汇票
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("MoneyOrder", PKeySigner.Mode.Normal, 5)]
        MoneyOrders,

        /// <summary>
        /// 背书转让
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("Endors", PKeySigner.Mode.Normal, 5)]
        Endorsements,

        /// <summary>
        /// 预收退款申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("RefundApply", PKeySigner.Mode.Normal, 5)]
        RefundApplies,

        /// <summary>
        /// 承兑汇票申请
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("AcceptApply", PKeySigner.Mode.Normal, 5)]
        AcceptanceApplies,

        /// <summary>
        /// 承兑汇票左表
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("AcceptLeft", PKeySigner.Mode.Normal, 5)]
        AcceptanceLefts,

        /// <summary>
        /// 承兑汇票右表
        /// </summary>
        [Repository(typeof(PvFinanceReponsitory))]
        [PKey("AcceptRight", PKeySigner.Mode.Normal, 5)]
        AcceptanceRights,
    }
}