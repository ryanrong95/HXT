using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ACTRequest
    {
        public ACTRequestHeader header { get; set; }

        public ACTRequestTxnInfoDetails txnInfoDetails { get; set; }
    }

    public class ACTRequestHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string timeStamp { get; set; }
        public string ctry { get; set; }
        public int noOfTxs { get; set; }
        public decimal totalTxnAmount { get; set; }       
    }

    public class ACTRequestTxnInfoDetails
    {
        public List<ACTRequestTxnInfo> txnInfo { get; set; }
    }
  
    public class ACTRequestTxnInfo
    {
        public string customerReference { get; set; }
        public string txnType { get; set; }
        public string txnDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }
        public string debitAccountCcy { get; set; }
        public string fxContractRef1 { get; set; }
        public decimal fxAmountUtilized1 { get; set; }
        public ACTSenderParty senderParty { get; set; }
        public ACTReceivingParty receivingParty { get; set; }
    }

    public class ACTSenderParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
        public string bankCtryCode { get; set; }
        public string swiftBic { get; set; }
    }

    public class ACTReceivingParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
    }
}
