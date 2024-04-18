using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.PvWsOrder.Services.XDTClientView;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    /// <summary>
    /// 付汇申请详情
    /// </summary>
    public class ApplyInfoViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请明细
        /// </summary>
        public Array ApplyItems { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public Array PIFiles { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商英文名
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public string Others { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 汇率类型
        /// </summary>
        public string ExchangeRateType { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 付汇总金额（人民币）
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public string SettlementDate { get; set; }

        /// <summary>
        /// 付汇委托书地址
        /// </summary>
        public string AgentTrustInstrumentURL { get; set; }

        /// <summary>
        /// 付汇委托书名称
        /// </summary>
        public string AgentTrustInstrumentName { get; set; }

        /// <summary>
        /// 付汇日期
        /// </summary>
        public string PayDate { get; set; }

        /// <summary>
        /// 是否是待审批状态
        /// </summary>
        public bool IsUpload { get; set; }

        /// <summary>
        /// 开户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 公司名
        /// </summary>
        public string AgentName { get; set; }
    }

    /// <summary>
    /// 待付汇申请
    /// </summary>
    public class ApplyViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        //order订单
        public List<PayExchangeApplyOrderViewModel> UnPayExchangeOrders { get; set; }

        /// <summary>
        /// PI文件列表
        /// </summary>
        public List<FileModel> PayExchangeApplyFiles { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 供应商银行
        /// </summary>
        public string SupplierBank { get; set; }

        /// <summary>
        /// 供应商银行
        /// </summary>
        public string SupplierBankName { get; set; }

        /// <summary>
        /// ABA（付美国必填）
        /// </summary>
        public string ABA { get; set; }

        /// <summary>
        /// IBAN（付欧盟必填）
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// 供应商银行地址
        /// </summary>
        public string SupplierBankAddress { get; set; }

        /// <summary>
        /// 供应商银行账号
        /// </summary>
        public string SupplierBankAccount { get; set; }

        /// <summary>
        /// 供应商银行代码
        /// </summary>
        public string SupplierBankCode { get; set; }

        /// <summary>
        /// 供应商银行地区
        /// </summary>
        public string SupplierBankRegion{ get; set; }

        /// <summary>
        /// 供应商银行ABA
        /// </summary>
        public string SupplierBankABA { get; set; }

        /// <summary>
        /// 期望付汇日期
        /// </summary>
        public DateTime? ExpectDate { get; set; }

        //付款方式
        public string PayType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 美元汇率
        /// </summary>
        public decimal USDRate { get; set; }

        /// <summary>
        /// 总金额（人民币）
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 总付汇金额
        /// </summary>
        public decimal TotalPayMoney { get; set; }

        /// <summary>
        /// 是否垫款 (0-垫款, 1-不垫款)
        /// </summary>
        public string IsAdvance { get; set; }

        /// <summary>
        /// 未使用的垫款额度
        /// </summary>
        public decimal UnUsedAdvanceMoney { get; set; }

        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public string HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal HandlingFee { get; set; }
    }

    /// <summary>
    /// 付汇申请订单模型
    /// </summary>
    public class PayExchangeApplyOrderViewModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 已付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 本次付汇金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }

        /// <summary>
        /// 可付汇金额
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// 报关总货值
        /// </summary>
        public decimal DeclarePrice { get; set; }

        public bool IsReadonly { get; set; }
    }

}