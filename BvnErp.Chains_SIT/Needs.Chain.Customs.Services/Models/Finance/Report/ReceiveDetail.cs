using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReceiveDetail : IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 合同
        /// </summary>
        public string ContrNo { get; set; }
        /// <summary>
        /// 增值税
        /// </summary>
        public decimal? AddedValueTax { get; set; }
        /// <summary>
        /// 消费税
        /// </summary>
        public decimal? ExciseTax { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal? Tariff { get; set; }
        /// <summary>
        /// 货款
        /// </summary>
        public decimal? GoodsAmount { get; set; }
        /// <summary>
        /// 代理费
        /// </summary>
        public decimal? AgencyFee { get; set; }
        /// <summary>
        /// 杂费
        /// </summary>
        public decimal? Incidental { get; set; }
        /// <summary>
        /// 显示用得代理费
        /// </summary>
        public decimal? ShowAgencyFee
        {
            get
            {
                return this.AgencyFee + this.Incidental;
            }
        }
        /// <summary>
        /// 付款汇率
        /// </summary>
        public decimal? PaymentExchangeRate { get; set; }
        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal? FCAmount
        {
            get
            {
                return this.GoodsAmount / this.PaymentExchangeRate;
            }
        }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }
        /// <summary>
        /// 应收账款-货款
        /// </summary>
        public decimal? DueGoods
        {
            get
            {
                return this.FCAmount * this.RealExchangeRate;
            }
        }

        /// <summary>
        /// 损益
        /// </summary>
        public decimal? Gains
        {
            get
            {
                return this.DueGoods - this.GoodsAmount;
            }
        }

        #region 查询条件
        public string FinanceReceiptID { get; set; }

        public OrderReceiptType Type { get; set; }
        #endregion

    }

    public class OrderReceiveDetail: ReceiveDetail
    {
        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiveDate { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal ReceiveAmount { get; set; }
        /// <summary>
        /// 已确认金额
        /// </summary>
        public decimal ClearAmount { get; set; }

        /// <summary>
        /// InvoiceTypeInt
        /// </summary>
        public int? InvoiceTypeInt { get; set; }

        /// <summary>
        /// 客户类型(单抬头、双抬头)
        /// </summary>
        public string InvoiceTypeName { get; set; }
    }

    public class PaymentExchangeRate:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
