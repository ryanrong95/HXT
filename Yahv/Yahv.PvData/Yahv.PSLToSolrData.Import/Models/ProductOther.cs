using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品其他信息扩展
    /// </summary>
    public class ProductOther
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        [JsonProperty("productID")]
        public string ProductID { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [JsonProperty("lanaguage")]
        public string Language { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }
    }
}
