using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooModelDetail : IUnique
    {

        /// <summary>
        /// 报关系统型号ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户系统型号ID
        /// </summary>
        public string CustomsUnionID { get; set; }


        public string ModelStatus { get; set; }

        public string InvoiceStatus { get; set; }

        public string ImportTaxCode { get; set; }

        public string AddedValueTaxCode { get; set; }





        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 委托单价
        /// </summary>
        public decimal AgentUnitPrice { get; set; }

        /// <summary>
        /// 委托总价
        /// </summary>
        public decimal AgentTotalPrice { get; set; }

        /// <summary>
        /// 申报单价
        /// </summary>
        public decimal DeclareUnitPrice
        {
            get
            {
                return (this.AgentTotalPrice * Needs.Ccs.Services.ConstConfig.TransPremiumInsurance / this.Qty).ToRound(4);
            }
        }

        /// <summary>
        /// 申报总价
        /// </summary>
        public decimal DeclareTotalPrice
        {
            get
            {
                return (this.AgentTotalPrice * Needs.Ccs.Services.ConstConfig.TransPremiumInsurance).ToRound(2);
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
        /// 报关总价（RMB）
        /// </summary>
        public decimal DeclTotalRMB
        {
            get
            {
                return (this.DeclareTotalPrice * this.CustomsExchangeRate).ToRound(0);
            }
        }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 应交关税
        /// </summary>
        public decimal TariffValue
        {
            get
            {
                return (this.DeclTotalRMB * this.TariffRate).ToRound(2);
            }
        }

        /// <summary>
        /// 实交关税率
        /// </summary>
        public decimal RealTariffRate { get; set; }

        /// <summary>
        /// 实交关税
        /// </summary>
        public decimal RealTariffValue
        {
            get
            {
                return (this.DeclTotalRMB * this.RealTariffRate).ToRound(2);
            }
        }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal AddTaxRate { get; set; }

        /// <summary>
        /// 应交增值税
        /// </summary>
        public decimal AddTaxValue
        {
            get
            {
                return ((this.DeclTotalRMB + TariffValue) * this.AddTaxRate).ToRound(2);
            }
        }

        /// <summary>
        /// 实交增值税率
        /// </summary>
        public decimal RealAddTaxRate { get; set; }

        /// <summary>
        /// 实交增值税
        /// </summary>
        public decimal RealAddTaxValue
        {
            get
            {
                return ((this.DeclTotalRMB + TariffValue) * this.RealAddTaxRate).ToRound(2);
            }
        }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 报关系统订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户系统订单ID
        /// </summary>
        public string CustomsOrderID { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryID { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 开票税点
        /// </summary>
        public decimal? InvoiceTaxRate { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }
    }
}
