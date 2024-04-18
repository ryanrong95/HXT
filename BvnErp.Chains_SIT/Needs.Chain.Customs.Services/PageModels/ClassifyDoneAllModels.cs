using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClassifyDoneAllModels : IUnique
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string ProductName { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public string HSCode { get; set; }

        public string Elements { get; set; }

        public decimal? TariffRate { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        public Enums.PreProductUserType UseType { get; set; }

        public Enums.ClassifyStatus ClassifyStatus { get; set; }

        public string ClientName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime CompleteTime { get; set; }

        public string ClassifyFirstOperatorName { get; set; }

        public string ClassifySecondOperatorName { get; set; }

        public string TaxCode { get; set; }

        public string TaxName { get; set; }

        public bool IsOrdered { get; set; }

        public IEnumerable<string> ClassifyLog { get; set; }
    }
}
