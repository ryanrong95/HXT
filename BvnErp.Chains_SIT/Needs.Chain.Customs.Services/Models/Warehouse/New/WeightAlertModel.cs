using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{ 
    public class WeightAlertModel
    {
        public decimal TotalWeight { get; set; }
        public List<WeightAlterItem> Items { get; set; }
    }

    public class WeightAlterItem
    {
        public string Model { get; set; }
        public decimal Qty { get; set; }
    }
}
