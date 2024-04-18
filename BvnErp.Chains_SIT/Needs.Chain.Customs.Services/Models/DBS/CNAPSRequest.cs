using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CNAPSRequest
    {
        public ACTRequestHeader header { get; set; }

        public CNAPSRequestTxnInfoDetails txnInfoDetails { get; set; }
    }

    public class CNAPSRequestTxnInfoDetails
    {
        public List<CNAPSRequestTxnInfo> txnInfo { get; set; }
    }

    public class CNAPSRequestTxnInfo
    {
        public string customerReference { get; set; }
        public string txnType { get; set; }
        public string txnDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }
        public string debitAccountCcy { get; set; }       
        public CNAPSSenderParty senderParty { get; set; }
        public CNAPSReceivingParty receivingParty { get; set; }
    }

    public class CNAPSSenderParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
        public string bankCtryCode { get; set; }
        public string swiftBic { get; set; }
    }

    public class CNAPSReceivingParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
        public string bankCtryCode { get; set; }
        public string bankName { get; set; }
    }
}
