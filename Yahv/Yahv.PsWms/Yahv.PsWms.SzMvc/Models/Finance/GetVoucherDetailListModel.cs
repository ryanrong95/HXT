using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetVoucherDetailListSearchModel
    {
        public string CutDateIndex { get; set; }
    }

    public class GetVoucherDetailListReturnModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string PayeeLeftCreateDateDes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderStatusDes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderCreateDateDes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConductDes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UnitPriceDes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalDes { get; set; }
    }
}