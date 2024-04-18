using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 产品库存
    /// </summary>
    public class ProductInventory : Linq.IUnique
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 产品
        /// </summary>
        [JsonProperty("productID")]
        public string ProductID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [JsonProperty("supplier")]
        public string Supplier { get; set; }

        /// <summary>
        /// 库存地
        /// </summary>
        [JsonProperty("storage")]
        public string Storage { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        [JsonProperty("dateCode")]
        public string DateCode { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        [JsonProperty("packaging")]
        public string Packaging { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sign")]
        public string Sign { get; set; }

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

        /// <summary>
        /// 库存量
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// Normal, Down
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        #region 扩展属性

        /// <summary>
        /// 产品交货条件
        /// </summary>
        [JsonProperty("productDelivery")]
        public ProductDelivery ProductDelivery { get; set; }

        /// <summary>
        /// 产品报价
        /// </summary>
        [JsonProperty("productQuotes")]
        public ProductQuote[] ProductQuotes { get; set; }

        #endregion

        #region 构造器

        internal ProductInventory()
        {
        }

        #endregion
    }
}
