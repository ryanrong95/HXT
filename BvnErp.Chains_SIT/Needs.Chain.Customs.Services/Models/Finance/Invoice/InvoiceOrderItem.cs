using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票产品项（仅用于开票申请界面展示）
    /// </summary>
    public class InvoiceOrderItem : OrderItem
    {
        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsRate { get; set; }

        /// <summary>
        /// 税率(客户补充协议)
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get { return this.Category.TaxName; } }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get { return this.Category.TaxCode; } }

        /// <summary>
        /// 销售单价（已加运保杂等费用）
        /// </summary>
        public decimal SalesUnitPrice
        {
            get
            {
                return (this.SalesTotalPrice / this.Quantity).ToRound(4);
            }
        }

        /// <summary>
        /// 销售总额
        /// </summary>
        public decimal SalesTotalPrice
        {
            get
            {
                return (this.SalesTotalPriceRat / (1 + this.InvoiceTaxRate)).ToRound(4);
            }
        }

        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal SalesUnitPriceRat
        {
            get { return (this.SalesTotalPriceRat / this.Quantity).ToRound(4); }
        }

        /// <summary>
        /// 含税总额
        /// </summary>
        public decimal SalesTotalPriceRat{ get; set; }

        public decimal GetSalesTotalPriceRat(IEnumerable<Order> orders, ClientAgreement agreement)
        {
            var order = orders.FirstOrDefault(o => o.ID == this.OrderID);
            int count = order.Items.Count();

            //是否平摊代理费
            var isave = order.AgencyFee <= agreement.MinAgencyFee ? true : false;
            var agencyFee = order.AgencyFee * (1 + agreement.InvoiceTaxRate);
            var aveAgencyFee = isave ? (agencyFee / count) : (this.TotalPrice * order.AgencyFeeExchangeRate * agreement.AgencyRate * (1 + agreement.InvoiceTaxRate));
            //平摊杂费（不含商检费）
            var miscFee = order.MiscFees.Sum(f => f.Count * f.UnitPrice * f.Rate) * (1 + agreement.InvoiceTaxRate);
            var aveMiscFee = (miscFee / count);
            //关税
            decimal ImportTax = this.ImportTax.Value ?? 0M;
            //增值税
            decimal AddedValueTax = this.AddedValueTax.Value ?? 0M;
            //商检费
            decimal InspectionFee = this.InspectionFee.GetValueOrDefault() * (1 + agreement.InvoiceTaxRate);
            this.SalesTotalPriceRat = (TotalPrice * order.ProductFeeExchangeRate + ImportTax + AddedValueTax + aveAgencyFee + InspectionFee + aveMiscFee).ToRound(4);

            return this.SalesTotalPriceRat;
        }

        public decimal GetSalesTotalPriceRatSpeed(IEnumerable<Order> orders, ClientAgreement agreement, List<InvoiceItemAmountHelp> helper)
        {
            var current = helper.Where(t => t.OrderID == this.OrderID).FirstOrDefault();
            var order = orders.FirstOrDefault(o => o.ID == this.OrderID);
            int count = current.OrderItemCount;

            //是否平摊代理费
            var isave = (current.AgencyFee <= agreement.MinAgencyFee ? true : false) || (order.OrderBillType == Enums.OrderBillType.Pointed);
            var agencyFee = current.AgencyFee * (1 + agreement.InvoiceTaxRate);
            var aveAgencyFee = isave ? (agencyFee / count) : (this.TotalPrice * order.AgencyFeeExchangeRate * agreement.AgencyRate * (1 + agreement.InvoiceTaxRate));
            //平摊杂费（不含商检费）
            var miscFee = current.MiscFees * (1 + agreement.InvoiceTaxRate);
            var aveMiscFee = (miscFee / count);

            //decimal ImportTax = this.ImportTax.Value ?? 0M;
            ////消费税
            //decimal ExciseTax = this.ExciseTax?.Value ?? 0M;
            ////增值税
            //decimal AddedValueTax = this.AddedValueTax.Value ?? 0M;

            //开票时，使用实收汇率计算税费 ryan  20211122
            var topPrice = (this.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
            var total = (topPrice * order.CustomsExchangeRate.Value).ToRound(2);
            //关税
            decimal ImportTax = (total * this.ImportTax.Rate).ToRound(2);
            decimal ImportReal = (total * this.ImportTax.ReceiptRate).ToRound(2);
            //消费税
            decimal ExciseTax = ((total + ImportTax) / (1 - this.ExciseTax.ReceiptRate) * this.ExciseTax.ReceiptRate).ToRound(2);
            var exciseTaxRate = this.ExciseTax.ReceiptRate;
            decimal AddedValueTax = (((total + ImportTax) + (total + ImportTax) / (1 - exciseTaxRate) * exciseTaxRate) * this.AddedValueTax.ReceiptRate).ToRound(2);

            //商检费
            decimal InspectionFee = this.InspectionFee.GetValueOrDefault() * (1 + agreement.InvoiceTaxRate);
            this.SalesTotalPriceRat = (TotalPrice * order.ProductFeeExchangeRate + ImportReal + ExciseTax + AddedValueTax + aveAgencyFee + InspectionFee + aveMiscFee).ToRound(4);

            return this.SalesTotalPriceRat;
        }

    }
}
