using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品文件
    /// </summary>
    public class ProductFile
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
        /// 文件类型
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createDate")]
        [JsonConverter(typeof(Converters.DateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonProperty("modifyDate")]
        [JsonConverter(typeof(Converters.DateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime ModifyDate { get; set; }
    }
}
