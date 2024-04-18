using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsOrder.WebApi.Models
{
    /// <summary>
    /// 归类结果
    /// </summary>
    public class ClassifiedResult
    {
        [JsonProperty(PropertyName = "ItemID")]
        public string ItemID { get; set; }
        [JsonProperty(PropertyName = "MainID")]
        public string MainID { get; set; }
        [JsonProperty(PropertyName = "ProductId")]
        public string ProductID { get; set; }


        [JsonProperty(PropertyName = "HSCodeId")]
        public string HSCodeID { get; set; }

        [JsonProperty(PropertyName = "CreatorID")]
        public string CreatorID { get; set; }

        [JsonProperty(PropertyName = "Step")]
        public string Step { get; set; }


        [JsonProperty(PropertyName = "OriginRate")]
        public decimal OriginRate { get; set; }

        [JsonProperty(PropertyName = "FVARate")]
        public decimal FVARate { get; set; }

        [JsonProperty(PropertyName = "Ccc")]
        public bool Ccc { get; set; }

        [JsonProperty(PropertyName = "Embargo")]
        public bool Embargo { get; set; }

        [JsonProperty(PropertyName = "HkControl")]
        public bool HkControl { get; set; }

        [JsonProperty(PropertyName = "Coo")]
        public bool Coo { get; set; }

        [JsonProperty(PropertyName = "CIQ")]
        public bool CIQ { get; set; }

        [JsonProperty(PropertyName = "CIQprice")]
        public decimal CIQprice { get; set; }

        [JsonProperty(PropertyName = "IsHighPrice")]
        public bool IsHighPrice { get; set; }

        [JsonProperty(PropertyName = "IsDisinfected")]
        public bool IsDisinfected { get; set; }

        [JsonProperty(PropertyName = "IsSysCcc")]
        public bool IsSysCcc { get; set; }

        [JsonProperty(PropertyName = "IsSysEmbargo")]
        public bool IsSysEmbargo { get; set; }
    }
}