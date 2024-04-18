using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReImportModel:IUnique
    {
        public string ID { get; set; }
        public string OrderRecepitID { get; set; }
        public string RequestID { get; set; }
        public InvoiceType InvoiceType { get; set; }
        /// <summary>
        /// 预收账款
        /// </summary>
        public decimal? PreMoney { get; set; }
        /// <summary>
        /// 汇兑损益
        /// </summary>
        public decimal? Diff { get; set; }
        /// <summary>
        /// 货款
        /// </summary>
        public decimal? GoodsMoney { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 增值税
        /// </summary>
        public decimal? AddTax { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal? Tariff { get; set; }
        /// <summary>
        /// 消费税
        /// </summary>
        public decimal? ExciseTax { get; set; }
        /// <summary>
        /// 代理费
        /// </summary>
        public decimal? Agency { get; set; }
        /// <summary>
        /// 凭证字
        /// </summary>
        public string ReCreWord { get; set; }
        /// <summary>
        /// 凭证号
        /// </summary>
        public string ReCreNo { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
