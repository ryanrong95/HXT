using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.Models;

namespace Needs.Wl.Client.Services
{
    public class ClassifiedPreProduct : Needs.Wl.Models.PreProduct
    {
        public string ProductName { get; set; }

        public string HSCode { get; set; }

        public decimal? TariffRate { get; set; }

        public decimal? AddedValueRate { get; set; }

        public decimal? ExciseTaxRate { get; set; }

        public string TaxCode { get; set; }

        public string TaxName { get; set; }

        public Wl.Models.Enums.ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        public string Unit1 { get; set; }

        public string Unit2 { get; set; }

        public string CIQCode { get; set; }

        public string Elements { get; set; }

        public Wl.Models.Enums.ClassifyStatus? ClassifyStatus { get; set; }

        public string ClassifyFirstOperator { get; set; }

        public string ClassifySecondOperator { get; set; }
    }
}