using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Enums;
using Needs.Linq;

namespace Needs.Ccs.Services.Models
{

    /// <summary>
    /// 订单收款明细
    /// </summary>
    public class OrderReceiptStats : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键ID/订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 下单时的会员补充协议
        /// </summary>
        internal ClientAgreement Agreement { get; set; }

        /// <summary>
        /// 开票税点
        /// </summary>
        private decimal TaxPoint
        {
            get
            {
                return 1 + this.Agreement.InvoiceTaxRate;
            }
        }

        private decimal productFeeExchangeRate;
        /// <summary>
        /// 货款汇率
        /// </summary>
        private decimal ProductFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.Agreement.ProductFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        productFeeExchangeRate = this.RealExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        productFeeExchangeRate = this.CustomsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        productFeeExchangeRate = this.Agreement.ProductFeeClause.ExchangeRateValue.HasValue ? this.Agreement.ProductFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        productFeeExchangeRate = 0;
                        break;
                }

                return this.productFeeExchangeRate;
            }
        }

        /// <summary>
        /// 海关汇率
        /// </summary>
        internal decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealExchangeRate { get; set; }

        /// <summary>
        /// 报关货值(外币)
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 报关货值(人民币)
        /// </summary>
        public decimal RMBDeclarePrice
        {
            get
            {
                return this.DeclarePrice * this.ProductFeeExchangeRate;
            }
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 应收款
        /// </summary>
        public decimal Receivable { get; set; }

        /// <summary>
        /// 已收款
        /// </summary>
        public decimal Received { get; set; }

        /// <summary>
        /// 欠款
        /// </summary>
        public decimal Overdraft { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }

        #region 实收

        /// <summary>
        /// 实收货款
        /// </summary>
        public decimal ProductReceived { get; set; }

        /// <summary>
        /// 实收关税
        /// </summary>
        public decimal TariffReceived { get; set; }

        /// <summary>
        /// 实收增值税
        /// </summary>
        public decimal AVTReceived { get; set; }

        /// <summary>
        /// 实收代理费
        /// </summary>
        public decimal AgencyReceived { get; set; }

        /// <summary>
        /// 实收杂费
        /// </summary>
        public decimal IncidentalReceived { get; set; }

        /// <summary>
        /// 实收香港现金费用(人民币)
        /// </summary>
        public decimal HKFeeReceived { get; set; }

        #endregion

        #region 实付
        /// <summary>
        /// 实付货款
        /// </summary>
        public decimal ProductPaid { get; set; }
        /// <summary>
        /// 实付关税
        /// </summary>
        public decimal TariffPaid { get; set; }
        /// <summary>
        /// 实付增值税
        /// </summary>
        public decimal AVTPaid { get; set; }

        /// <summary>
        /// 实付杂费
        /// </summary>
        public decimal IncidentalPaid { get; set; }

        #endregion

        #region 利润

        /// <summary>
        /// 实收合计
        /// </summary>
        public decimal TaxGeneratTotal
        {
            get
            {
                return this.TariffReceived + this.AVTReceived + this.AgencyReceived / TaxPoint + this.IncidentalReceived / TaxPoint + this.HKFeeReceived;
            }
        }

        /// <summary>
        /// 实付合计
        /// </summary>
        public decimal FeeTotal
        {
            get
            {
                return this.TariffReceivable + this.AVTReceivable + this.IncidentalPaid;
            }
        }

        /// <summary>
        /// 利润
        /// </summary>
        public decimal Profit
        {
            get
            {
                return this.TaxGeneratTotal - this.FeeTotal;
            }
        }

        #endregion

        #region 应收  

        // 应收增值税，关税= 实付增值税，关税
        /// <summary>
        /// 应收货款
        /// </summary>
        public decimal Product { get; set; }


        /// <summary>
        /// 应收关税
        /// </summary>
        public decimal TariffReceivable { get; set; }

        /// <summary>
        /// 应收增值税
        /// </summary>
        public decimal AVTReceivable { get; set; }

        /// <summary>
        /// 应收代理费
        /// </summary>
        public decimal AgencyReceivable { get; set; }

        /// <summary>
        /// 应收杂费
        /// </summary>
        public decimal IncidentalReceivable { get; set; }

        #endregion

        #region 应收款日期

        private DateTime productFeeDueDate;
        /// <summary>
        /// 货款应收款日期
        /// </summary>
        public DateTime ProductFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.ProductFeeClause.PeriodType;
                var dateUsed = this.DDate.HasValue ? DDate.Value : this.CreateDate;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        productFeeDueDate = dateUsed;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.ProductFeeClause.DaysLimit.Value;
                        productFeeDueDate = dateUsed.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.ProductFeeClause.MonthlyDay.Value;
                        productFeeDueDate = new DateTime(dateUsed.Year, dateUsed.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        productFeeDueDate = dateUsed;
                        break;
                }

                return this.productFeeDueDate;
            }
        }

        private DateTime taxFeeDueDate;
        /// <summary>
        /// 税款应收款日期
        /// </summary>
        public DateTime TaxFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.TaxFeeClause.PeriodType;
                var dateUsed = this.DDate.HasValue ? DDate.Value : this.CreateDate;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        taxFeeDueDate = dateUsed;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.TaxFeeClause.DaysLimit.Value;
                        taxFeeDueDate = dateUsed.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.TaxFeeClause.MonthlyDay.Value;
                        taxFeeDueDate = new DateTime(dateUsed.Year, dateUsed.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        taxFeeDueDate = dateUsed;
                        break;
                }

                return this.taxFeeDueDate;
            }
        }

        private DateTime agencyFeeDueDate;
        /// <summary>
        /// 代理费应收款日期
        /// </summary>
        public DateTime AgencyFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.AgencyFeeClause.PeriodType;
                var dateUsed = this.DDate.HasValue ? DDate.Value : this.CreateDate;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        agencyFeeDueDate = dateUsed;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.AgencyFeeClause.DaysLimit.Value;
                        agencyFeeDueDate = dateUsed.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.AgencyFeeClause.MonthlyDay.Value;
                        agencyFeeDueDate = new DateTime(dateUsed.Year, dateUsed.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        agencyFeeDueDate = dateUsed;
                        break;
                }

                return this.agencyFeeDueDate;
            }
        }

        /// <summary>
        /// 杂费应收款日期
        /// </summary>
        private DateTime incidentalFeeDueDate;
        public DateTime IncidentalFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.IncidentalFeeClause.PeriodType;
                var dateUsed = this.DDate.HasValue ? DDate.Value : this.CreateDate;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        incidentalFeeDueDate = dateUsed;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.IncidentalFeeClause.DaysLimit.Value;
                        incidentalFeeDueDate = dateUsed.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.IncidentalFeeClause.MonthlyDay.Value;
                        incidentalFeeDueDate = new DateTime(dateUsed.Year, dateUsed.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        incidentalFeeDueDate = dateUsed;
                        break;
                }

                return this.incidentalFeeDueDate;
            }
        }

        #endregion

        #region 付款日期
        /// <summary>
        /// 货款付款日期
        /// </summary>
        public DateTime? ProductPayDate { get; set; }
        /// <summary>
        /// 关税付款日期
        /// </summary>
        public DateTime? TariffPayDate { get; set; }
        /// <summary>
        /// 增值税付款日期
        /// </summary>
        public DateTime? AVTPayDate { get; set; }
        /// <summary>
        /// 杂费付款日期
        /// </summary>
        public DateTime? IncidentalPayDate { get; set; }
        #endregion

        #region 综合管理 未收款用

        public string PeriodType { get; set; }

        public string MonthlyDay { get; set; }

        public string DaysLimit { get; set; }

        #endregion

        #endregion
        public OrderReceiptStats()
        {

        }
    }

    public class OrderReceiptInfo : IUnique
    {
        public OrderReceiptInfo() { }

        public string ID { get; set; }

        public string ClientCode { get; set; }

        public string CompanyName { get; set; }

        public Enums.OrderStatus OrderStatus { get; set; }

        public DateTime? DeclareDate { get; set; }

        public decimal DeclarePrice { get; set; }//.ToString("0.00")

        public decimal Received { get; set; }

        public decimal Profit { get; set; }

        public string Salesman { get; set; }


    }

    public class OrderReceiptStatViewModel
    {
        public string ClientCode { get; set; }
        public string CompanyName { get; set; }
        public string OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string DeclareDate { get; set; }
        public string DeclarePrice { get; set; }
        public decimal Receivable { get; set; }
        public decimal Received { get; set; }
        public decimal Overdraft { get; set; }
        public string Salesman { get; set; }
        public string Merchandiser { get; set; }
        public string PeriodType { get; set; }
        public string PeriodTypeDesc { get; set; }
        public string SettlementDate { get; set; }
        public int? ExceedDays { get; set; }
    }

}



