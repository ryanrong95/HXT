using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeclarationNoticeItemView
    {
        public string PackingID { get; set; }
        public string SortingID { get; set; }
        public string CaseNumber { get; set; }
        public string Batch { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Origin { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public decimal GrossWeight { get; set; }
        public string AdminName { get; set; }
        public DateTime PackingDate { get; set; }
        public DeclareNoticeItemStatus Status { get; set; }

    }
}
