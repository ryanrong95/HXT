using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BillSummary : IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 主订单编号
        /// </summary>
        public string MainOrderID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 报关合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 货值
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 货值(RMB)
        /// </summary>
        public decimal? RMBDeclarePrice { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal? AgencyRate { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal? AddedValueTax { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 代理费
        /// </summary>
        public decimal? AgencyFee { get; set; }

        /// <summary>
        /// 杂费
        /// </summary>
        public decimal? Incidental { get; set; }

        /// <summary>
        /// 税费合计
        /// </summary>
        public decimal? TotalTariff { get; set; }

        /// <summary>
        /// 报关总金额 
        /// </summary>
        public decimal? TotalDeclare { get; set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 交货供应商
        /// </summary>
        public string  SupplierName { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }
    }
}
