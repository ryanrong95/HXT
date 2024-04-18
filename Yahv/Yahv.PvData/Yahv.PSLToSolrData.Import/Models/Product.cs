using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PSLToSolrData.Import.Models
{
    /// <summary>
    /// 标准产品
    /// </summary>
    public class Product : Linq.IUnique
    {
        /// <summary>
        /// 主键
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [JsonProperty("partNumber")]
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        [JsonProperty("packageCase")]
        public string PackageCase { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createDate")]
        [JsonConverter(typeof(Converters.DateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime CreateDate { get; set; }

        #region 扩展属性

        /// <summary>
        /// 产品分类
        /// </summary>
        [JsonProperty("productCategory")]
        public ProductCategory ProductCategory { get; set; }

        /// <summary>
        /// 产品其他信息扩展
        /// </summary>
        [JsonProperty("productOthers")]
        public ProductOther[] ProductOthers { get; set; }

        /// <summary>
        /// 产品文件
        /// </summary>
        [JsonProperty("productFiles")]
        public ProductFile[] ProductFiles { get; set; }

        /// <summary>
        /// 产品库存
        /// </summary>
        [JsonProperty("productInventories")]
        public ProductInventory[] ProductInventories { get; set; }

        #endregion

        #region 构造器

        internal Product()
        {
        }

        #endregion
    }
}
