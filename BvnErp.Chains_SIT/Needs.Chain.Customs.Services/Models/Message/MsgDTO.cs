using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MsgDTO
    {
        public string clientCode { get; set; }
        public int systemID { get; set; }
        public int spotType { get; set; }
        public string orderID { get; set; }
        public string expressNo { get; set; }
    }
}
