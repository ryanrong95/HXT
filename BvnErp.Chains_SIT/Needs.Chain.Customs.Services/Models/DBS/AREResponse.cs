using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AREResponse
    {
        public AREHeader header { get; set; }
        public ARETxnEnqResponse txnEnqResponse { get; set; }
    }

    public class ARETxnEnqResponse
    {
        public string enqStatus { get; set; }
        public string enqRejectCode { get; set; }
        public string enqStatusDescription { get; set; }
        public List<AREAcctInfo> acctInfo { get; set; }
    }

    public class AREAcctInfo
    {
        public string accountNo { get; set; }
        public string accountCcy { get; set; }
        public string availableBal { get; set; }
        public List<AREInitiatingParty> initiatingParty { get; set; }
    }

    public class AREInitiatingParty
    {
        public string name { get; set; }
        public List<ARETxnInfo> txnInfo { get; set; }
    }

    public class ARETxnInfo
    {
        public string drCrInd { get; set; }
        public string txnCode { get; set; }
        public string txnDesc { get; set; }
        public string txnDate { get; set; }
        public string valueDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }        
    }
}
