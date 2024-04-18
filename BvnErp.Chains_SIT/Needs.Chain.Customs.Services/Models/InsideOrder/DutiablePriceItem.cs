using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DutiablePriceItem : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 申报项ID，作为芯达通财务入库库存ID
        /// </summary>
        public string DecListID { get; set; }

        /// <summary>
        /// 芯达通进价，不含运保杂，最后一个型号用减法计算
        /// </summary>
        public decimal InPrice { get; set; }

        /// <summary>
        /// 单据号
        /// </summary>
        public string ProductUniqueCode { get; set; }
        /// <summary>
        /// 完税价格
        /// </summary>
        public decimal DutiablePrice { get; set; }
       
        /// <summary>
        /// 型号信息分类
        /// </summary>
        public string TaxName { get; set; }
        /// <summary>
        /// 型号信息分类值
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 报关完成日期
        /// </summary>
        public string DeclareDate { get; set; }
        /// <summary>
        /// 报关单号
        /// </summary>
        public string EntryId { get; set; }
      
        /// <summary>
        /// 规格型号E
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 数量G
        /// </summary>
        public decimal  Qty{ get; set; }
        
        public string OrderItemID { get; set; }
        public string DecHeadID { get; set; }
        /// <summary>
        /// 应交关税
        /// </summary>
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 应交增值税
        /// </summary>
        public decimal AddedValueRate { get; set; }
        /// <summary>
        /// 应交消费税
        /// </summary>
        public decimal ConsumeTaxRate { get; set; }
        /// <summary>
        /// 实交关税
        /// </summary>
        public decimal TariffReceiptRate { get; set; }
        /// <summary>
        /// 实交增值税
        /// </summary>
        public decimal AddedValueReceiptRate { get; set; }
        /// <summary>
        /// 实交消费税
        /// </summary>
        public decimal? ConsumeTaxReceiptRate { get; set; }
        public string Origin { get; set; }
        public string Manfacture { get; set; }
        public string ProductName { get; set; }
        public string HSCode { get; set; }
        public string Supplier { get; set; }
        public decimal DeclTotal { get; set; }
        public decimal DeclTotalRMB { get; set; }
        /// <summary>
        /// 应交关税(RMB)
        /// </summary>
        public decimal TariffPay { get; set; }
        /// <summary>
        /// 应交增值税
        /// </summary>
        public decimal ValueVat { get; set; }
        /// <summary>
        /// 应交消费税
        /// </summary>
        public decimal ExciseTax { get; set; }

        /// <summary>
        /// 海关进价
        /// </summary>
        public decimal CusInputPrice { get; set; }
    }
}
