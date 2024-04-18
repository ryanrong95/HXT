using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CgDelcareSZPrice
    {
        public List<CgDelcareSZPriceItem> Items { get; set; }
    }

    public class CgDelcareSZPriceItem
    {
        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        public string OrderItemID { get; set; }

        public decimal InUnitPrice { get; set; }

        public decimal OutUnitPrice { get; set; }
    }


    public class SZPrice: IUnique
    {

        public string ID { get; set; }
        public string OrderItemID { get; set; }
        /// <summary>
        /// 大订单号
        /// </summary>
        public string MainOrderId { get; set; }
        /// <summary>
        ///订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 报关货值
        /// </summary>
        public decimal DeclTotal { get; set; }
        /// <summary>
        /// 海关汇率
        /// </summary>

        public decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 实收的关税率
        /// </summary>
        public decimal ReceiptRate { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        public decimal diffence { get; set; }
        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal TaxAmount { get; set; }

        //public List<string> OrderIds { get; set; }



    }
}
