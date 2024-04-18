using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ACTACK1Response
    {
        public ACTRequestHeader header { get; set; }
        public List<ACTACK1ResponseTxnResponses> txnResponses { get; set; }
    }

    public class ACTACK1ResponseTxnResponses
    {
        public string responseType { get; set; }
        public string msgRefId { get; set; }
        public string txnStatus { get; set; }
        public string txnRejectCode { get; set; }
        public string txnStatusDescription { get; set; }
    }
}
