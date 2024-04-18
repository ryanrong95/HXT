using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.CusFileListen.Models
{
    public class ManifestListViewModel
    {
        public string ID { get; set; }
        public string VoyageNo { get; set; }
        public string BillNo { get; set; }
        public string Port { get; set; }
        public string PackNo { get; set; }
        public string AgentName { get; set; }
        public string CreateDate { get; set; }
        public string StatusName { get; set; }
    }
}
