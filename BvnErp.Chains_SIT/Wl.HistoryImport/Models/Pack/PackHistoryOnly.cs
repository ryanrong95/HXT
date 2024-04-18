using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class PackHistoryOnly
    {
        public string ID { get; set; }
        public string BoxIndex { get; set; }     
        public decimal GrossWeight { get; set; }
        public decimal Quantity { get; set; }
        public List<PackItemHistoryOnly> PackItems { get; set; }
    }
}
