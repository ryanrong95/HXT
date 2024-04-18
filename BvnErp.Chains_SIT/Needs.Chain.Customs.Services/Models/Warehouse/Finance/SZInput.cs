using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 深圳销项信息
    /// </summary>
    public class SZInput : DecList
    {
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票公司
        /// </summary>
        public string InvoiceCompany { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string ConsignorCode { get; set; }

        /// <summary>
        /// 报关状态
        /// </summary>
        public string CusDecStatus { get; set; }

        /// <summary>
        /// 是否外单
        /// </summary>
        public bool IsExternalOrder { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsRate { get; set; }

        /// <summary>
        /// 报关总价（RMB）
        /// </summary>
        public decimal DeclTotalRMB
        {
            get
            {
                int decimalForTotalPrice = 0;
                DateTime dt20201126 = Convert.ToDateTime("2020-11-26");
                DateTime dt20210127 = Convert.ToDateTime("2021-01-27");
                if (this.OrderCreateDate >= dt20210127)
                {
                    decimalForTotalPrice = 2;
                }
                else if (this.DecHeadDDate != null )

                {
                    if (this.DecHeadDDate > dt20201126 && this.DecHeadDDate < dt20210127)
                    {
                        if (!string.IsNullOrEmpty(this.DecHeadType) && this.DecHeadType.Length >= 3)
                        {
                            if (this.DecHeadType.Substring(2, 1).Equals("3"))
                            {
                                decimalForTotalPrice = 2;
                            }
                        }
                    }
                    else if(this.DecHeadDDate >= dt20210127)
                    {
                        decimalForTotalPrice = 2;
                    }
                    
                }

                return (this.DeclTotal * this.CustomsRate).ToRound(decimalForTotalPrice);
            }
        }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 实际缴税关税率
        /// </summary>
        public decimal TariffRatePaid { get; set; }

        /// <summary>
        /// 应交关税(RMB)
        /// </summary>
        public decimal TariffPay
        {
            get
            {
                return (this.DeclTotalRMB * this.TariffRate).ToRound(2);
            }
        }

        /// <summary>
        /// 总关税（报关单）
        /// </summary>
        public decimal TotalTariff { get; set; }

        /// <summary>
        /// 实交关税(RMB)(当总关税小于50RMB时为0；否则等于应交关税)
        /// </summary>
        public decimal TariffPayed
        {
            get
            {
                //if (this.TotalTariff >= 50)
                //{
                //    return TariffPay;
                //}
                //else
                //{
                //    return 0M;
                //}

                return (this.DeclTotalRMB * this.TariffRatePaid).ToRound(2);
            }
        }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal Vat { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal ValueVat
        {
            get
            {
                //增值税计算公式：Round(((完税价 + 关税) + (完税价 + 关税) / (1-消费税税率) * 消费税税率) * 增值税率, 2)
                return (((this.DeclTotalRMB + TariffPay) + (this.DeclTotalRMB + this.TariffPay) / (1 - this.ExciseTaxRate) * this.ExciseTaxRate) * this.Vat).ToRound(2);
            }
        }

        /// <summary>
        /// 总增值税（报关单）
        /// </summary>
        public decimal TotalValueVat { get; set; }

        /// <summary>
        /// 实交增值税(RMB)(当总增值税小于50RMB时为0；否则等于应交增值税)
        /// </summary>
        public decimal ValueVatPayed
        {
            get
            {
                if (this.TotalValueVat >= 50)
                {
                    return ValueVat;
                }
                else
                {
                    return 0M;
                }
            }
        }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 应交消费税
        /// </summary>
        public decimal ExciseTax
        {
            get
            {
                //消费税计算公式：Round((完税价格＋关税)÷(1－消费税税率)×消费税税率, 2）
                return ((this.DeclTotalRMB + this.TariffPay) / (1 - this.ExciseTaxRate) * this.ExciseTaxRate).ToRound(2);
            }
        }

        /// <summary>
        /// 总消费税（报关单）
        /// </summary>
        public decimal TotalExciseTax { get; set; }

        /// <summary>
        /// 实交消费税(RMB)(当总消费税小于50RMB时为0；否则等于应交消费税)
        /// </summary>
        public decimal ExciseTaxPayed
        {
            get
            {
                if (this.TotalExciseTax >= 50)
                {
                    return this.ExciseTax;
                }
                else
                {
                    return 0M;
                }
            }
        }

        /// <summary>
        /// 完税价格(报关总价 + 应交关税 + 应交消费税)
        /// 2023-03-08 鲁亚慧 完税价格应该包含消费税
        /// </summary>
        public decimal CustomsValue
        {
            get
            {
                return this.DeclTotalRMB + this.TariffPay + this.ExciseTax;
            }
        }

        /// <summary>
        /// 完税价格增值税(完税价格 * 0.16)
        /// </summary>
        public decimal CustomsValueVat
        {
            get
            {
                //return (CustomsValue * this.Vat).ToRound(2);
                return (((this.DeclTotalRMB + TariffPay) + (this.DeclTotalRMB + this.TariffPay) / (1 - this.ExciseTaxRate) * this.ExciseTaxRate) * this.Vat).ToRound(2);
            }
        }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 大赢家订单编号
        /// </summary>
        public string DYJOrderID { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public string DecHeadType { get; set; }
        public DateTime? DecHeadDDate { get; set; }
        public DateTime? OrderCreateDate { get; set; }

        //增值税缴款书填发日期
        public DateTime? FillinDate { get; set; }
    }
}