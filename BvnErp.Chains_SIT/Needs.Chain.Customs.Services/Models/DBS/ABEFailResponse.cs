using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ABEFailResponse
    {
        public ABEFailHeader header { get; set; }
        public FailAccountBalResponse accountBalResponse { get; set; }

    }

    public class ABEFailHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public DateTime timeStamp { get; set; }
    }

    public class FailAccountBalResponse
    {
        public string enqStatus { get; set; }
        public string enqRejectCode { get; set; }
        public string enqStatusDescription { get; set; }
    }

}
