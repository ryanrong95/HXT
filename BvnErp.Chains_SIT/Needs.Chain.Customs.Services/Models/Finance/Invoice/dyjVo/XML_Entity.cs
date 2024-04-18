using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class XML_Entity
    {
        public string XML标识 { get; set; }
        public string XML { get; set; }
    }

    public class InVItemJoinDeclist
    { 
        public string InvoiceNoticeItemID { get; set; }

        public string DeclistItemID { get; set; }

        public string OrderItemID { get; set; }

        public decimal Quantity { get; set; }
    }

    public class VItemJoinDecList4Allocate 
    {
        public string InvoiceNoticeXmlItemID { get; set; }
        public string DeclistItemID { get; set; }
        public decimal Quantity { get; set; }
    }
}
