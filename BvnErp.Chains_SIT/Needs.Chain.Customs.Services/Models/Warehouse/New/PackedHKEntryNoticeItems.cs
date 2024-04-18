using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PackedHKEntryNoticeItem
    {
        public string ID { get; set; }
        public string PackingID { get; set; }
        public string OrderID { get; set; }
        public string CaseNo { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal OrderItemQty { get; set; }
        public decimal RelQty { get; set; }
        public string CarrierName { get; set; }
        public string WaybillNo { get; set; }
    }
}
