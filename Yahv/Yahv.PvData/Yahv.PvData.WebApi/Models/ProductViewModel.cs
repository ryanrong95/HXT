using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvData.WebApi.Models
{
    /// <summary>
    /// 产品报价结果
    /// </summary>
    public class QuotedResult
    {
        /// <summary>
        /// 型号
        /// </summary>
        [JsonProperty(PropertyName = "PartNumber")]
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        [JsonProperty(PropertyName = "Manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        [JsonProperty(PropertyName = "Origin")]
        public string Origin { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [JsonProperty(PropertyName = "UnitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [JsonProperty(PropertyName = "Quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        [JsonProperty(PropertyName = "CIQprice")]
        public decimal CIQprice { get; set; }
    }
}