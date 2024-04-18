using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.CusFileListen.Models
{
    public class DeclareListViewModel
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string ContrNo { get; set; }
        public string BillNo { get; set; }
        public string EntryId { get; set; }
        public string PreEntryId { get; set; }
        public string AgentName { get; set; }
        public string IsInspection { get; set; }
        public string InputerID { get; set; }
        public string CreateDate { get; set; }
        public string StatusName { get; set; }
    }
}
