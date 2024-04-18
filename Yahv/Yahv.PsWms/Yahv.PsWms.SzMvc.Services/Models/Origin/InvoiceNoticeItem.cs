using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class InvoiceNoticeItem:IUnique
    {
        public string ID { get; set; }
        public string InvoiceNoticeID { get; set; }
        public string BillID { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal? Difference { get; set; }
        public string InvoiceNo { get; set; }
        public GeneralStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }
    }
}
