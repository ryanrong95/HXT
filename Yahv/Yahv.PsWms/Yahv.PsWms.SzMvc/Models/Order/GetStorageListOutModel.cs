using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetStorageListOutModel
    {
        /// <summary>
        /// page
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// rows
        /// </summary>
        public int rows { get; set; }

        /// <summary>
        /// 订单状态 Int
        /// </summary>
        public string OrderStatusInt { get; set; }

        /// <summary>
        /// 下单时间开始时间
        /// </summary>
        public string CreateDateBegin { get; set; }

        /// <summary>
        /// 下单时间结束时间
        /// </summary>
        public string CreateDateEnd { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }
    }
}