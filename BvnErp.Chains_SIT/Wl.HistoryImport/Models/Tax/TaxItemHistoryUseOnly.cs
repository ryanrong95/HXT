using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
     public class TaxItemHistoryUseOnly
    {
        public string OrderItemID { get; set; }       
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal Difference { get; set; }
        public string OrderNO { get; set; }

    }
}
