using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户预归类产品信息
    /// </summary>
    public class ClientPreProduct : IUnique
    {
        public string ID { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string ProductName { get; set; }
        public string HSCode { get; set; }
        public decimal? TariffRate { get; set; }
        public decimal? AddedValueRate { get; set; }
        public decimal? ExciseTaxRate { get; set; }
        public string TaxCode { get; set; }

        public string TaxName { get; set; }
        public ItemCategoryType? Type { get; set; }
        public decimal? InspectionFee { get; set; }
        public string Unit1 { get; set; }
        public string Unit2 { get; set; }
        public string CIQCode { get; set; }
        public string Elements { get; set; }
        public ClassifyStatus ClassifyStatus { get; set; }
        public Status Status { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
        public string ClassifyFirstOperator { get; set; }
        public string ClassifySecondOperator { get; set; }
        public string ClientID { get; set; }
        public decimal Price { get; set; }
        public string ProductUnionCode { get; set; }
        public string Currency { get; set; }

        public string Supplier { get; set; }

    }
}
