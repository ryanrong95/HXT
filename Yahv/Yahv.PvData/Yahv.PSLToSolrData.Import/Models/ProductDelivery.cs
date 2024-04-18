using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品交货条件
    /// </summary>
    public class ProductDelivery
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 最小起订量
        /// </summary>
        [JsonProperty("moq")]
        public int? Moq { get; set; }

        /// <summary>
        /// 跳跃量
        /// </summary>
        [JsonProperty("jump")]
        public int? Jump { get; set; }

        /// <summary>
        /// 最小起订金额
        /// </summary>
        [JsonProperty("mpa")]
        public decimal? Mpa { get; set; }

        /// <summary>
        /// 最小起订金额币种
        /// </summary>
        [JsonProperty("mpaCurrency")]
        public int? MpaCurrency { get; set; }

        /// <summary>
        /// 货期
        /// </summary>
        [JsonProperty("delivery")]
        public string Delivery { get; set; }

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
