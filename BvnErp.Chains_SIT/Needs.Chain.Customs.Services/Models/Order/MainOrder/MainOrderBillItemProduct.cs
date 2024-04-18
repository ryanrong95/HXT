using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   
    public class MainOrderBillItemProduct
    {      
        public string ProductName { get; set; }
        public string Model { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TariffRate { get; set; }
        public decimal TotalCNYPrice { get; set; }
        public decimal? Traiff { get; set; }
        public decimal? ExciseTax { get; set; }
        public decimal? AddedValueTax { get; set; }
        public decimal AgencyFee { get; set; }
        public decimal IncidentalFee { get; set; }
        public decimal MyProperty { get; set; }
        public decimal? InspectionFee { get; set; }
    }
}
