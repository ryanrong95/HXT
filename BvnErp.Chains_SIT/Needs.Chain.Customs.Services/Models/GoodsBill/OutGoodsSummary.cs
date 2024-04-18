using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public  class OutGoodsSummary
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        /// <summary>
        /// 完税价格
        /// </summary>
        public decimal? TaxedPrice { get; set; }
        /// <summary>
        /// 开票价格
        /// </summary>
        public decimal? InvoicePrice { get; set; }

        public decimal declTotal { get; set; }
        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }
        public string OrderItemID { get; set; }
        public DateTime? StorageDate { get; set; }

    }
}
