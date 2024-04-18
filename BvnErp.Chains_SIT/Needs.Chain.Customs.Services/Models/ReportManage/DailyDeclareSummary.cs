using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DailyDeclareSummary : IUnique
    {
        public string ID { get; set; }
        public string ContrNo { get; set; }
        public decimal GQty { get; set; }
        public decimal DeclTotal { get; set; }
        public string OwnerName { get; set; }
        public int PackNo { get; set; }
        public decimal NetWt { get; set; }
        public decimal GrossWt { get; set; }
        public bool IsInspection { get; set; }
        public bool? IsQuarantine { get; set; }
    }
}
