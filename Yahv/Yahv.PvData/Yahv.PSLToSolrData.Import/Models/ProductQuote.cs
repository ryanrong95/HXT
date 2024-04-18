using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品报价
    /// </summary>
    public class ProductQuote
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public long ID { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [JsonProperty("inventoryID")]
        public string InventoryID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("moq")]
        public int Moq { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        [JsonProperty("currency")]
        public int Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [JsonProperty("unitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createDate")]
        [JsonConverter(typeof(Converters.DateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否当前报价
        /// </summary>
        [JsonProperty("isCurrent")]
        public bool? IsCurrent { get; set; }

        /// <summary>
        /// 100:原价,200:现价（折扣价）
        /// </summary>
        [JsonProperty("type")]
        public int? Type { get; set; }
    }
}
