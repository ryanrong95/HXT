using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品分类
    /// </summary>
    public class ProductCategory
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 分类类型
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        [JsonProperty("nameCN")]
        public string NameCN { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        [JsonProperty("nameEN")]
        public string NameEN { get; set; }

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
