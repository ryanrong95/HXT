using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum FinanceType
    {
        [Description("收款")]
        Receipt = 1,

        [Description("付款")]
        Payment = 2,
    }

    /// <summary>
    /// 费用类型
    /// </summary>
    public enum FinanceFeeType
    {
        [Description("货款")]
        Product = 1,

        [Description("关税")]
        Tariff = 2,

        [Description("增值税")]
        AddedValueTax = 3,

        [Description("杂费")]
        Incidental = 4,

        [Description("手续费")]
        Poundage = 5,

        [Description("退款")]
        Refund = 6,

        [Description("资金往来")]
        FundExchange = 7,

        [Description("费用")]
        Fee = 8,

        [Description("税金")]
        Tax = 9,

        [Description("资金调拨")]
        FundTransfer = 10,

        [Description("借款")]
        Loan = 11,     

        [Description("还款")]
        Repayment = 12,

        [Description("预收账款")]
        DepositReceived = 13,

        [Description("银行利息")]
        BankInterest = 14,

        /// <summary>
        /// 账户流水导入时不明确的收款费用类型
        /// </summary>
        [Description("供应链其他收入")]
        Other = 15,

        [Description("报关服务费")]
        DeclareService = 16,

        [Description("仓储收入")]
        Warehouse = 17,

        [Description("代发货收入")]
        GoodsAgent = 18,

        [Description("异常理货收入")]
        SpecialPack = 19,

        [Description("租赁收入")]
        Rent = 20,
        [Description("银行借款")]
        BankBorrow = 21,
        [Description("存款利息")]
        Interest = 22,
        [Description("单位借款")]
        CompanyBorrow = 23,
        [Description("收回押金")]
        Deposit = 24,
        [Description("员工还款")]
        EmployeePayback = 25,
        [Description("调入")]
        CallIn = 26,
        [Description("综合其他收入")]
        GenereOther = 27,
        [Description("汇兑损益")]
        ExchangeLoss = 28,
        [Description("代销收款")]
        SaleAgent = 29,
        [Description("认证保证金")]
        Guarantee = 30,
        [Description("第三方收款")]
        ThirdService = 31,

        #region 付款-供应链业务
        [Description("付款-预付账款")]
        PayPreBill = 1101,
        [Description("付款-货款")]
        PayGoods = 1102,
        [Description("付款-海关增值税")]
        PayAddedValue = 1103,
        [Description("付款-关税")]
        PayTariff = 1104,
        [Description("付款-缴纳所得税")]
        PayIncomeTax = 1105,
        #endregion


        #region 付款-综合业务
        [Description("付款-押金")]
        PayDespoist = 1201,
        [Description("付款-银行还贷")]
        PayBank = 1202,
        [Description("付款-还单位借款")]
        PayCompany = 1203,
        [Description("付款-员工借款")]
        PayEmployee = 1204,
        [Description("付款-其他支出")]
        PayOther = 1205,
        [Description("付款-调出")]
        PayCallout = 1206,
        [Description("付款-代销付款")]
        PaySaleAgent = 1207,
        [Description("付款-代销退款")]
        PaySaleAgentBack = 1208,
        [Description("付款-退定金")]
        PayDespoistBack = 1209,
        #endregion

        #region 付款-综合业务-费用      
        [Description("付款-职工薪酬")]
        PaySalary = 1301,
        [Description("付款-劳动保险费")]
        PayEndowment = 1302,
        [Description("付款-住房公积金")]
        PayHousingProvidentFund = 1303,
        [Description("付款-运杂费")]
        PayIncidentals = 1304,
        [Description("付款-差旅费")]
        PayBussiness = 1305,
        [Description("付款-广告宣传费")]
        PayAdvertisement = 1306,
        [Description("付款-租赁费及物业费")]
        PayPropertyMan = 1307,
        [Description("付款-业务招待费")]
        PayEntertain = 1308,
        [Description("付款-借款利息")]
        PayInterest = 1309,
        [Description("付款-报关服务费")]
        PayService = 1310,
        [Description("付款-仓储保管费")]
        PayWarehouse = 1311,
        [Description("付款-电话及网络通信费")]
        PayNet = 1312,
        [Description("付款-包装费")]
        PayPackage = 1313,
        [Description("付款-审计及会计服务费")]
        PayAudit = 1314,
        [Description("付款-税金及附加")]
        PayTax = 1315,
        [Description("付款-贸易通清关费")]
        PayTrade = 1316,
        [Description("付款-印花税")]
        PayStampTax = 1317,
        [Description("付款-房产税")]
        PayHouse = 1318,
        [Description("付款-残保金")]
        PayInjury = 1319,
        [Description("付款-借款附加费")]
        PayBorrowAdded = 1320,
        [Description("付款-银行手续费")]
        PayBankPaypal = 1321,
        [Description("付款-贴现利息")]
        PayAcceptance = 1322,
        [Description("付款-存款利息")]
        PayDepositInterest = 1323,
        [Description("付款-内部利息")]
        PayInterInterest = 1324,
        [Description("付款-水电费")]
        PayWater = 1325,
        [Description("付款-诉讼费")]
        PayLawsuit = 1326,
        [Description("付款-坏账损失")]
        PayBadBorrow = 1327,
        [Description("付款-办公用品及设备购置费")]
        PayOffice = 1328,
        [Description("付款-车辆支出")]
        PayCar = 1329,
        [Description("付款-其他费用")]
        PayOtherFee = 1330,
        #endregion

    }

    /// <summary>
    /// 付款申请状态
    /// </summary>
    public enum PaymentApplyStatus
    {
        [Description("待审批")]
        Auditing = 1,
        [Description("已审批")]
        Audited = 2,
        [Description("已取消")]
        Canceled = 3,
        [Description("已完成")]
        Completed = 4
    }

    /// <summary>
    /// 付款通知状态
    /// </summary>
    public enum PaymentNoticeStatus
    {
        [Description("待付款")]
        UnPay = 1,
        [Description("已付款")]
        Paid = 2,
        [Description("已取消")]
        Canceled = 3
    }

    /// <summary>
    /// 订单收款费用类型
    /// </summary>
    public enum OrderFeeType
    {
        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        Product = 1,

        /// <summary>
        /// 关税
        /// </summary>
        [Description("关税")]
        Tariff = 2,

        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税")]
        AddedValueTax = 3,

        /// <summary>
        /// 代理费
        /// </summary>
        [Description("服务费")]
        AgencyFee = 4,

        /// <summary>
        /// 杂费
        /// </summary>
        [Description("杂费")]
        Incidental = 5,

        /// <summary>
        /// 消费税
        /// </summary>
        [Description("消费税")]
        ExciseTax = 6,
    }

    /// <summary>
    /// 订单收款状态
    /// </summary>
    public enum OrderReceivedStatus
    {
        [Description("未收款")]
        UnReceive = 1,

        [Description("部分收款")]
        PartReceived = 2,

        [Description("已收款")]
        Received = 3
    }

    /// <summary>
    /// 订单应收/实收类型
    /// </summary>
    public enum OrderReceiptType
    {
        /// <summary>
        /// 应收
        /// </summary>
        [Description("应收")]
        Receivable = 1,

        /// <summary>
        /// 实收
        /// </summary>
        [Description("实收")]
        Received = 2
    }

    /// <summary>
    /// 开票通知文件类型
    /// </summary>
    public enum InvoiceNoticeFileType
    {
        /// <summary>
        /// 发票
        /// </summary>
        [Description("发票")]
        Invoice = 1,
    }

    /// <summary>
    /// 付汇供应商信息类型（应付/实付）
    /// </summary>
    public enum PaySupplierPayType
    {
        /// <summary>
        /// 应付
        /// </summary>
        [Description("应付")]
        Payable = 1,

        /// <summary>
        /// 实付
        /// </summary>
        [Description("实付")]
        Payment = 2,
    }

    /// <summary>
    /// OrderReceipts 在 SwapNotices 中的使用类型
    /// </summary>
    public enum ReceiptUseType
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        UnHandle = 1,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Handled = 2,

        /// <summary>
        /// 结果数据
        /// </summary>
        [Description("结果数据")]
        ResultData = 3,
    }

    /// <summary>
    /// 账户的基本类型 基本户 一般户
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 基本户
        /// </summary>
        [Description("基本账户")]
        basic = 1,

        /// <summary>
        /// 一般户
        /// </summary>
        [Description("一般账户")]
        normal = 2,

        /// <summary>
        /// 现金账户
        /// </summary>
        [Description("现金账户")]
        cash = 4,

        /// <summary>
        /// 银行承兑账户
        /// </summary>
        [Description("银行承兑账户")]
        bank = 8,

        /// <summary>
        /// 商业承兑账户
        /// </summary>
        [Description("商业承兑账户")]
        business = 16,

        /// <summary>
        /// 微信账户
        /// </summary>
        [Description("微信账户")]
        wechat = 32,

        /// <summary>
        /// 支付宝账户
        /// </summary>
        [Description("支付宝账户")]
        alipay = 64,

        /// <summary>
        /// 离岸账户
        /// </summary>
        [Description("离岸账户")]
        offshore = 128
    }

    /// <summary>
    /// 账户来源 标准 简易
    /// </summary>
    public enum AccountSource
    {
        /// <summary>
        /// 标准
        /// </summary>
        [Description("标准")]
        standard = 1,

        /// <summary>
        /// 简易
        /// </summary>
        [Description("简易")]
        easy = 2,
    }

    /// <summary>
    /// 调拨类型
    /// </summary>
    public enum FundTransferType
    {
        /// <summary>
        /// 借款
        /// </summary>
        [Description("借款")]
        Loan = 1,

        /// <summary>
        /// 货款
        /// </summary>
        [Description("货款")]
        ProductFee =2,

        /// <summary>
        /// 调账
        /// </summary>
        [Description("调账")]
        Transfer =3,

        /// <summary>
        /// 购汇
        /// </summary>
        [Description("购汇")]
        ForeignExchangePurchasing = 4,

        /// <summary>
        /// 还款
        /// </summary>
        [Description("还款")]
        Repayment = 5,

        /// <summary>
        /// 贴现
        /// </summary>
        [Description("贴现")]
        Discount = 6,

        /// <summary>
        /// 资金往来
        /// </summary>
        [Description("资金往来")]
        Transaction = 7,
    }

}
