using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PartNoReceiveItem
    {
        public string origin { get; set; }
        public decimal gw { get; set; }
        public string mfr { get; set; }
        public string unit { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public decimal nw { get; set; }
        public string pn { get; set; }
        public string orgCarton { get; set; }
        public int qty { get; set; }
        public string sale_orderline_id { get; set; }
        public string tax_code { get; set; }
        public string hs_code { get; set; }

    }
}
