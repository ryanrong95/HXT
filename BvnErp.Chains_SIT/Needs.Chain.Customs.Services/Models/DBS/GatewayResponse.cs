using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GatewayResponse
    {
        public GatewayHeader header { get; set; }
        public GatewayError error { get; set; }
    }

    public class GatewayHeader
    {
        public string msgId { get; set; }
        public string timeStamp { get; set; }
    }

    public class GatewayError
    {
        public string code { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }
}
