using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 业务员的利润提成
    /// </summary>
    public class Profit : IUnique
    {
        /// <summary>
        /// 主键ID/业务员ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 业务员名称
        /// </summary>
        public string Name { get; set; }

        public decimal ReferrerCommission { get; set; }

        public decimal TotalOrderProfit { get; set; }

        public decimal TotalSalesCommission { get; set; }
        /// <summary>
        /// 利润提成明细
        /// </summary>
        public IEnumerable<ProfitDetail> ProfitDetails { get; set; }

        public string DepartmentCode { get; set; }

        public Profit()
        {

        }
    }

    /// <summary>
    ///订单提成明细
    /// </summary>
    public class ProfitDetail : IUnique
    {
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

        /// <summary>
        /// 货款汇率
        /// </summary>
        private decimal productFeeExchangeRate;
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
        /// 单抬头的客户 收取货值千一作为成本
        /// </summary>
        public decimal ThousandthTax
        {
            get
            {
                if (this.Client.Agreement.InvoiceType == Enums.InvoiceType.Full)
                {
                    return RMBDeclarePrice * ConstConfig.ThousandthTaxRate;
                }
                else
                {
                    return 0M;
                }
            }
        }

        /// <summary>
        /// 减免金额（超期未付汇金额 小于50，汇率误差），减免 千一的税赋
        /// </summary>
        public decimal ReductionThousandthTax
        {
            get 
            {
                if (this.UnPayExchangeAmount < 50)
                {
                    return this.ThousandthTax;
                }
                else
                {
                    return 0M;
                }
            }
        }

        /// <summary>
        /// 超90天未换汇金额
        /// </summary>
        public decimal UnPayExchangeAmount 
        { 
            get 
            {
                return this.Client.UnPayExchangeAmount.Value;
            }
        
        }

        /// <summary>
        /// 超120天未换汇金额
        /// </summary>
        public decimal UnPayExchangeAmount4M
        {
            get 
            {
                return this.Client.UnPayExchangeAmount4M.Value;
            }
        }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealExchangeRate { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        #region 税代合计

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal AgencyRate
        {
            get
            {
                return this.Agreement.AgencyRate;
            }
        }

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
        /// 不含税的实收代理费
        /// </summary>
        public decimal AgencyReceivedUnTax
        {
            get { return this.AgencyReceived / TaxPoint; }
        }

        /// <summary>
        /// 实收杂费
        /// </summary>
        public decimal IncidentalReceived { get; set; }

        /// <summary>
        /// 不含税的实收杂费
        /// </summary>
        public decimal IncidentalReceivedUnTax
        {
            get { return this.IncidentalReceived / TaxPoint; }
        }

        /// <summary>
        /// 香港现金费用(人民币)
        /// </summary>
        public decimal HKFeeReceived { get; set; }

        /// <summary>
        /// 税代合计
        /// </summary>
        public decimal TaxGeneratTotal
        {
            get
            {
                return this.TariffReceived + this.AVTReceived + this.AgencyReceivedUnTax + this.IncidentalReceivedUnTax + this.HKFeeReceived;
            }
        }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiveDate { get; set; }

        #endregion

        #region 费用合计

        /// <summary>
        /// 应收关税
        /// </summary>
        public decimal TariffReceivable { get; set; }

        /// <summary>
        /// 应收增值税
        /// </summary>
        public decimal AVTReceivable { get; set; }

        /// <summary>
        /// 实付杂费
        /// </summary>
        public decimal IncidentalPaid { get; set; }

        /// <summary>
        /// 费用合计
        /// </summary>
        public decimal FeeTotal
        {
            get
            {
                return this.TariffReceivable + this.AVTReceivable + this.IncidentalPaid;
            }
        }

        #endregion

        #region 利润提成

        /// <summary>
        /// 订单利润
        /// </summary>
        public decimal OrderProfit
        {
            get
            {
                //税代费实收 - 费用合计(成本) - 香港库房收费 - 千一税赋 + 满足免条件的千一税赋
                return this.TaxGeneratTotal - this.FeeTotal - this.HKFeeReceived - this.ThousandthTax + this.ReductionThousandthTax;
            }
        }

        /// <summary>
        /// 总利润
        /// </summary>
        public decimal TotalProfit
        {
            get { return this.TaxGeneratTotal - this.FeeTotal - this.ThousandthTax + this.ReductionThousandthTax; }
        }
        /// <summary>
        /// 比例
        /// </summary>
        public decimal proportion { get; set; }

        /// <summary>
        /// 业务提成
        /// </summary>
        public decimal Commission
        {
            get
            {

                return this.TotalProfit * this.proportion;
            }
        }

        /// <summary>
        /// 业务提成
        /// </summary>
        public decimal SalesCommission
        {
            get
            {
                using (var view = new Views.ClientAdminsView())
                {
                    var count = view.Where(item => item.ClientID == this.Client.ID && item.Type == Enums.ClientAdminType.Referrer && item.Status == Enums.Status.Normal).Count();
                    if (count == 0)
                    {
                        return (this.TotalProfit * this.proportion).ToRound(2);
                    }
                    else
                    {
                        return (this.TotalProfit * this.proportion * ConstConfig.ProfitDiscount).ToRound(2);
                    }
                }
            }
        }
        /// <summary>
        ///引荐人业务提成
        /// </summary>
        public decimal ReferrerCommission
        {
            get
            {
                using (var view = new Views.ClientAdminsView())
                {
                    var count = view.Where(item => item.ClientID == this.Client.ID && item.Type == Enums.ClientAdminType.Referrer && item.Status == Enums.Status.Normal).Count();
                    if (count != 0)
                    {
                        return this.TotalProfit * this.proportion * (1 - ConstConfig.ProfitDiscount);
                    }
                    else
                    {
                        return 0;
                    }

                }
            }
        }

        /// <summary>
        /// 跟单提成
        /// </summary>
        public decimal MerchandiserCommission
        {
            get
            {
                using (var view = new Views.ClientAdminsView())
                {
                    var count = view.Where(item => item.ClientID == this.Client.ID && item.Type == Enums.ClientAdminType.Merchandiser && item.Status == Enums.Status.Normal).Count();
                    if (count == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        //当月海关汇率 = list<订单>.where(currency = USD).first().CustomsExchangeRate
                        var CustomsExchangeRate = new Views.OrdersView().Where(item => item.Currency == "USD").First().CustomsExchangeRate;
                        var NewCustomsExchangeRate = CustomsExchangeRate == null ? 0 : CustomsExchangeRate;
                        return (this.DeclarePrice * this.RealExchangeRate - 500000 * (decimal)NewCustomsExchangeRate) * Convert.ToDecimal(0.0001);
                    }
                }
            }
        }
        #endregion

        #region 开票信息

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType? InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal? InvoiceTaxRate { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        public ProfitInvoiceInfo ProfitInvoiceInfo { get; set; }

        #endregion

        /// <summary>
        /// 部门 DepartmentCode
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public DateTime FeeClauseDate { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public Enums.PeriodType FeeClauseType { get; set; }
    }

    public class ProfitInvoiceInfo
    {
        public string OrderID { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        public string InvoiceNo { get; set; }
    }

    public class ProfitFeeClause
    {
        public string OrderID { get; set; }

        public DateTime PeriodDate { get; set; }
    }
}
