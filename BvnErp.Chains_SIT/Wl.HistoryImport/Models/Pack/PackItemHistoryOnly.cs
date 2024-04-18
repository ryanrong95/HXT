using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class PackItemHistoryOnly
    {
        public string PackID { get; set; }
        public string Model { get; set; }
        public string PlaceOfProduction { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GroosWeight { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Brand { get; set; }
        public string ModelID { get; set; }
    }
}
