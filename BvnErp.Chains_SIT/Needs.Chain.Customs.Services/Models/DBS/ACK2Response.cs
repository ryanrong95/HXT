using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ACK2Response
    {
        public ACTRequestHeader header { get; set; }

        public List<ACK2ResponseTxnResponses> txnResponses { get; set; }
    }

    public class ACK2ResponseTxnResponses
    {
        public string responseType { get; set; }
        public string customerReference { get; set; }
        public string msgRefId { get; set; }
        public string txnRefId { get; set; }
        public string txnType { get; set; }
        public string txnStatus { get; set; }
        public string txnRejectCode { get; set; }
        public string txnStatusDescription { get; set; }
    }
}
