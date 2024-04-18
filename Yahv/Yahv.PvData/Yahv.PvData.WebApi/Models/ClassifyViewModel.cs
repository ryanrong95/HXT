using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApi.Models
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

        [JsonProperty(PropertyName = "ClientName")]
        public string ClientName { get; set; }

        [JsonProperty(PropertyName = "ClientCode")]
        public string ClientCode { get; set; }

        [JsonProperty(PropertyName = "OrderedDate")]
        public DateTime OrderedDate { get; set; }

        [JsonProperty(PropertyName = "PIs")]
        public string PIs { get; set; }

        [JsonProperty(PropertyName = "CallBackUrl")]
        public string CallBackUrl { get; set; }


        string partNumber;
        [JsonProperty(PropertyName = "PartNumber")]
        public string PartNumber
        {
            get { return this.partNumber; }
            set { this.partNumber = value.FixSpecialChars(); }
        }

        string manufacturer;
        [JsonProperty(PropertyName = "Manufacturer")]
        public string Manufacturer
        {
            get { return this.manufacturer; }
            set { this.manufacturer = value.FixSpecialChars(); }
        }

        [JsonProperty(PropertyName = "Origin")]
        public string Origin { get; set; }

        [JsonProperty(PropertyName = "Currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "UnitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty(PropertyName = "Quantity")]
        public decimal Quantity { get; set; }


        [JsonProperty(PropertyName = "HSCode")]
        public string HSCode { get; set; }

        [JsonProperty(PropertyName = "CustomName")]
        public string CustomName { get; set; }

        [JsonProperty(PropertyName = "TariffName")]
        public string TariffName { get; set; }

        [JsonProperty(PropertyName = "TaxCode")]
        public string TaxCode { get; set; }

        [JsonProperty(PropertyName = "TaxName")]
        public string TaxName { get; set; }

        [JsonProperty(PropertyName = "Unit")]
        public string Unit { get; set; }

        [JsonProperty(PropertyName = "LegalUnit1")]
        public string LegalUnit1 { get; set; }

        [JsonProperty(PropertyName = "LegalUnit2")]
        public string LegalUnit2 { get; set; }

        [JsonProperty(PropertyName = "VATRate")]
        public decimal VATRate { get; set; }

        [JsonProperty(PropertyName = "ImportPreferentialTaxRate")]
        public decimal ImportPreferentialTaxRate { get; set; }

        [JsonProperty(PropertyName = "OriginRate")]
        public decimal OriginRate { get; set; }

        [JsonProperty(PropertyName = "ExciseTaxRate")]
        public decimal ExciseTaxRate { get; set; }

        [JsonProperty(PropertyName = "CIQCode")]
        public string CIQCode { get; set; }

        string elements;
        [JsonProperty(PropertyName = "Elements")]
        public string Elements
        {
            get { return this.elements; }
            set { this.elements = value.FixSpecialChars(); }
        }

        [JsonProperty(PropertyName = "Summary")]
        public string Summary { get; set; }

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

        [JsonProperty(PropertyName = "IsCustomsInspection")]
        public bool IsCustomsInspection { get; set; }


        [JsonProperty(PropertyName = "Step")]
        public string Step { get; set; }

        [JsonProperty(PropertyName = "CreatorID")]
        public string CreatorID { get; set; }

        [JsonProperty(PropertyName = "CreatorName")]
        public string CreatorName { get; set; }

        [JsonProperty(PropertyName = "Role")]
        public string Role { get; set; }


        [JsonProperty(PropertyName = "CreateDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "UpdateDate")]
        public DateTime UpdateDate { get; set; }
    }

    /// <summary>
    /// 待锁定产品
    /// </summary>
    public class TobeLockedProduct
    {
        [JsonProperty(PropertyName = "ItemID")]
        public string ItemID { get; set; }

        [JsonProperty(PropertyName = "MainID")]
        public string MainID { get; set; }

        [JsonProperty(PropertyName = "PartNumber")]
        public string PartNumber { get; set; }
    }
}