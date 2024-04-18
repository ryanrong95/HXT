using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Ccs.Services.Models
{
    public class ModelDetail : IUnique
    {
        /// <summary>
        /// 报关系统型号ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户系统型号ID
        /// </summary>
        public string CustomsUnionID { get; set; }

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
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

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
        public decimal DeclareUnitPrice { get; set; }

        /// <summary>
        /// 申报总价
        /// </summary>
        public decimal DeclareTotalPrice { get; set; }

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
                int decimalForTotalPrice = 0;

                DateTime dt20201126 = Convert.ToDateTime("2020-11-26");
                if (this.DeclareDate != null && this.DeclareDate > dt20201126)
                {
                    if (!string.IsNullOrEmpty(this.DecHeadType) && this.DecHeadType.Length >= 3)
                    {
                        if (this.DecHeadType.Substring(2, 1).Equals("3"))
                        {
                            decimalForTotalPrice = 2;
                        }
                    }
                }

                return (this.DeclareTotalPrice * this.CustomsExchangeRate).ToRound(decimalForTotalPrice);
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
        public DateTime DeclareDate { get; set; }

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
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 是否推送icgoo(是否有效)
        /// </summary>
        public int IsValid { get; set; }
        public string DecHeadType { get; set; }

        /// <summary>
        /// 客户公司名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 关税-缴款书税号
        /// </summary>
        public string CusTariffNumber { get; set; }

        /// <summary>
        /// 关税-缴款书金额
        /// </summary>
        public decimal CusTariffAmount { get; set; }

        /// <summary>
        /// 增值税-缴款书税号
        /// </summary>
        public string CusAddTaxNumber { get; set; }

        /// <summary>
        /// 增值税-缴款书金额
        /// </summary>
        public decimal CusAddTaxAmount { get; set; }

        //未税价：是否双抬头  ?  (DeclTotalRMB + 应交关税)   :   (DeclTotalRMB + 关税 + 服务费(分摊))
        public decimal? NoTaxAmount { get; set; }

        //服务费（分摊）
        public decimal? ServicePrice { get; set; }
    }
}