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
    public class SZOutput : SZExitNoticeItem
    {
        /// <summary>
        /// 开票公司
        /// </summary>
        public string InvoiceCompany { get; set; }

        /// <summary>
        /// 是否外单
        /// </summary>
        public bool IsExternalOrder { get; set; }

        /// <summary>
        /// 订单总数量
        /// </summary>
        public decimal TotalQuantity { get; set; }

        /// <summary>
        /// 进价（单价）
        /// </summary>
        public decimal InUnitPrice { get; set; }

        /// <summary>
        /// 销价（单价）
        /// </summary>
        public decimal OutUnitPrice { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal CostAmount
        {
            get
            {
                return this.InUnitPrice * this.Quantity;
            }
        }

        /// <summary>
        /// 销售金额
        /// </summary>
        public decimal SalesAmount
        {
            get
            {
                return this.OutUnitPrice * this.Quantity;
            }
        }

        /// <summary>
        /// 利率
        /// </summary>
        public decimal InterestRate
        {
            get
            {
                return ((this.OutUnitPrice * this.InUnitPrice) - 1).ToRound(4);
            }
        }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxRate
        {
            get
            {
                return ConstConfig.ValueAddedTaxRate;
            }
        }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal TaxAmount
        {
            get
            {
                return (InterestRate * SalesAmount).ToRound(2);
            }
        }

        /// <summary>
        /// 总额
        /// </summary>
        public decimal TotalAmount
        {
            get
            {
                return this.SalesAmount + this.TaxAmount;
            }
        }

        /// <summary>
        /// 总利率
        /// </summary>
        public decimal TotalInterestRate { get; set; }
    }
}