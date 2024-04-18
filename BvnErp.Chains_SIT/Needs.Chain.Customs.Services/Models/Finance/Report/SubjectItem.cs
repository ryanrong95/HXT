using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SubjectReportItem : IUnique
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 供应商（香港公司）
        /// </summary>
        public string ConsignorCode { get; set; }

        /// <summary>
        /// 报关外币总金额（含1.002） 
        /// ryan 20210122 卢晓玲需求
        /// </summary>
        public decimal DecForeignTotal { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        ///  报关总价 外币
        /// </summary>
        public decimal DecTotalPrice { get; set; }

        /// <summary>
        /// (6)	库存-应交关税：报关单的应交关税
        /// TODO，与已报关产品应交关税合计一样
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 报关单的实缴消费税（缴费流水中）
        /// </summary>
        public decimal? ActualExciseTax { get; set; }

        /// <summary>
        /// (7)	报关单的实缴增值税（缴费流水中）
        /// </summary>
        public decimal? ActualAddedValueTax { get; set; }

        /// <summary>
        /// (10)	报关当天实时汇率： TODO：汇率日志取实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// (15)	报关单实交关税
        /// </summary>
        public decimal? ActualTariff { get; set; }

        /// <summary>
        /// ClientAgreementID
        /// </summary>
        public string ClientAgreementID { get; set; }

        /// <summary>
        /// InvoiceType
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 客户类型(单抬头、双抬头)
        /// </summary>
        public string InvoiceTypeName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        public string ClientID { get; set; }



        #region 可以通关计算得出

        /// <summary>
        /// 报关委托总金额（报关外币总金额/1.002） 
        /// ryan 20210122 卢晓玲需求
        /// TODO:使用报关单页面的委托金额
        /// </summary>
        public decimal DecAgentTotal
        {
            get
            {
                //return (this.DecForeignTotal / ConstConfig.TransPremiumInsurance).ToRound(2);
                return DecTotalPrice;
            }
        }

        /// <summary>
        /// 报关运保杂总金额（报关外币总金额 - 报关委托总金额） 
        /// ryan 20210122 卢晓玲需求
        /// </summary>
        public decimal DecYunBaoZaTotal
        {
            get
            {
                return this.DecForeignTotal - this.DecAgentTotal;
            }
        }

        /// <summary>
        /// TODO:
        /// (3)	报关总价：已报关产品列表，型号报关总价(RMB)合计（两位小数）
        /// </summary>
        public decimal DecTotalPriceRMB
        {
            //get
            //{
            //    decimal realRate = this.CustomsExchangeRate == null ? 0 : this.CustomsExchangeRate.Value;
            //    return Math.Round(this.DecTotalPrice * realRate, 2);
            //}
            get; set;
        }

        /// <summary>
        /// (4)	库存-进口：Round(报关总价/1.002,2)
        /// </summary>
        public decimal ImportPrice
        {
            get
            {
                return Math.Round(this.DecTotalPriceRMB / 1.002M, 2);
            }
        }

        /// <summary>
        /// (5)	销售费用-运保杂：报关总价 减去 库存-进口
        /// </summary>
        public decimal SalePrice
        {
            get
            {
                return this.DecTotalPriceRMB - ImportPrice;
            }
        }

        /// <summary>
        /// (11)	应付-客户外币：委托金额(两位)
        /// 小订单外币金额
        /// </summary>
        public decimal DueCustomerFC
        {
            get
            {
                return this.DecTotalPrice;
            }
        }

        /// <summary>
        /// (12)	应付-客户RMB：Round(应付客户外币 * 报关当天实时汇率,2)
        /// </summary>
        public decimal DueCustomerRMB
        {
            get
            {
                decimal realRate = this.RealExchangeRate == null ? 0 : this.RealExchangeRate.Value;
                return Math.Round(this.DueCustomerFC * realRate, 2);
            }
        }

        /// <summary>
        /// (13)	应付-芯达通外币：（运保杂外币） = 报关金额外币 - 委托金额外币
        /// 报关金额包含1.002
        /// </summary>
        public decimal DueXDTFC
        {
            get
            {
                //鲁亚慧：科目明细表Q列（应付芯达通外币）应该等于E列（运保杂外币）； 2022-08-08 QQ反馈
                //return this.DueCustomerFC * 0.002M;
                return DecYunBaoZaTotal;
            }
        }

        /// <summary>
        /// (14)	应付-芯达通RMB：Round(应付-芯达通外币 * 报关当天实时汇率，2)
        /// </summary>
        public decimal DueXDTRMB
        {
            get
            {
                //R列=Q列*报关当日实时汇率
                decimal realRate = this.RealExchangeRate == null ? 0 : this.RealExchangeRate.Value;
                return Math.Round(this.DueXDTFC * realRate, 2);
            }
        }


        /// <summary>
        /// (16)	完税价格：Round（外币货值*海关汇率,0）+  应交关税。备注：用于数据确认：完税价格 = 库存进口 + 运保杂 + 应交关税；
        /// TODO：型号的美金*汇率，取两位小数？取整？ + 应交关税
        /// </summary>
        public decimal DutiablePrice
        {
            //get
            //{
            //    decimal customsRate = this.CustomsExchangeRate == null ? 0 : this.CustomsExchangeRate.Value;
            //    decimal tariff = this.Tariff == null ? 0 : this.Tariff.Value;
            //    return (Math.Round(DueCustomerFC * customsRate, 2) + tariff);
            //}
            get; set;
        }

        /// <summary>
        /// (8)	汇兑-客户： = 应付客户RMB  +  报关单实交关税  - 库存进口 - 库存应交关税    
        /// </summary>
        public decimal ExchangeCustomer
        {
            get
            {
                decimal actualTariff = this.ActualTariff == null ? 0 : this.ActualTariff.Value;
                decimal tariff = this.Tariff == null ? 0 : this.Tariff.Value;
                return this.DueCustomerRMB + actualTariff - this.ImportPrice - tariff;
            }
        }

        /// <summary>
        /// (9)	汇兑-芯达通： = 应付芯达通RMB  -  销售费用运保杂
        /// </summary>
        public decimal ExchangeXDT
        {
            get
            {
                return this.DueXDTRMB - this.SalePrice;
            }
        }


        #endregion


    }


    /// <summary>
    /// 报关进口（科目明细汇总表）
    /// </summary>
    public class SubjectReportStatistics
    {
        /// <summary>
        /// 报关ID，用于批量标记做账凭证（报关进口）
        /// </summary>
        public string DecHeadIDs { get; set; }

        //报关日期
        public string DeclareDate { get; set; }

        //天
        public string Tian
        {
            get
            {
                return DeclareDate.Substring(DeclareDate.Length - 2, 2);
            }
        }

        //报关总价，不显示
        public decimal DecTotalPriceRMB { get; set; }

        //委托报关（RMB)
        public decimal ImportPrice { get; set; }

        //运保杂
        public decimal YunBaoZa
        {
            get
            {
                return DecTotalPriceRMB - ImportPrice;
            }
        }

        //应交关税RMB
        public decimal Tariff { get; set; }

        //实交关税RMB
        public decimal ActualTariff { get; set; }

        //消费税，占位
        public string ExciseTax
        {
            get { return string.Empty; }
        }

        //消费税实缴
        public decimal ActualExciseTax { get; set; }

        //增值税RMB
        public decimal ActualAddedValueTax { get; set; }

        //合计委托外币
        public decimal TotalAgentAmount { get; set; }

        //委托金额-汇兑
        //Round（客户当天的外币合计  * 实时汇率，2） +  报关单实交关税（合计）  -  库存进口（合计） - 库存应交关税（合计）
        public decimal ExchangeCustomer
        {
            get
            {
                return Math.Round(TotalAgentAmount * RealExchangeRate, 2, MidpointRounding.AwayFromZero) + ActualTariff - ImportPrice - Tariff;
            }

        }

        //公司
        public string ClientName { get; set; }

        //运保杂-汇兑
        //=Q2+T2+F2-C2-D2-E2-J2
        public decimal ExchangeXDT
        {
            get {
                return Math.Round(DecAgentTotal * RealExchangeRate, 2, MidpointRounding.AwayFromZero) + Math.Round(DecYunBaoZaTotal * RealExchangeRate, 2, MidpointRounding.AwayFromZero) + ActualTariff
                    - ImportPrice - YunBaoZa - Tariff - ExchangeCustomer;
            }
        }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        //汇率
        public decimal RealExchangeRate { get; set; }

        //委托金额usd
        public decimal DecAgentTotal { get; set; }

        //物流方公司
        public string ConsignorCode { get; set; }

        //报关金额（不显示）
        public decimal DecForeignTotal { get; set; }

        //运保杂-usd
        public decimal DecYunBaoZaTotal { get; set; }

        //客户类型
        public string InvoiceTypeName { get; set; }

        //检验
        public decimal Check
        {

            get
            {
                return ImportPrice + YunBaoZa + Tariff + ActualAddedValueTax + ExchangeCustomer + ExchangeXDT - DecAgentTotal * RealExchangeRate - DecYunBaoZaTotal * RealExchangeRate - ActualTariff - ActualAddedValueTax;
            }
        }

        public string Addition1 { get { return string.Empty; } }

        public string Tian1
        {
            get { return Tian; }
        }

        public decimal DecAgentTotal1
        {
            get { return DecAgentTotal; }
        }

        public decimal ActualTariff1
        {
            get { return ActualTariff; }
        }

        public string Addition2 { get { return string.Empty; } }

        //委托金额-汇兑
        public decimal ExchangeCustomerOpposite
        {
            get
            {
                return -ExchangeCustomer;
            }

        }

        public decimal ImportPrice1
        {
            get { return ImportPrice; }
        }

        public decimal Tariff1
        {
            get { return Tariff; }
        }

        public string Addition3 { get { return string.Empty; } }

        public string ClientName1
        {
            get { return ClientName; }
        }

        public decimal RealExchangeRate1
        {
            get { return RealExchangeRate; }
        }

        //检验1
        public decimal Check1
        {
            get
            {
                //U委托金额usd * AC实时汇率 + V实交关税RMB + X委托金额-汇兑 - Y委托报关（RMB) - Z应交关税RMB
                return Math.Round(DecAgentTotal * RealExchangeRate + ActualTariff - ExchangeCustomer - ImportPrice - Tariff, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}
