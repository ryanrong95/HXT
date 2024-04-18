using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ACK3Response
    {
        public ACTRequestHeader header { get; set; }

        public List<ACK3ResponseTxnResponses> txnResponses { get; set; }
    }

    public class ACK3ResponseTxnResponses
    {
        public string responseType { get; set; }
        public string customerReference { get; set; }
        public string msgRefId { get; set; }
        public string txnRefId { get; set; }
        public string bankReference { get; set; }
        public string txnType { get; set; }
        public string txnStatus { get; set; }
        public string txnRejectCode { get; set; }
        public string txnStatusDescription { get; set; }

        public string txnCcy { get; set; }
        public decimal? txnAmount { get; set; }
        public string debitAccountCcy { get; set; }
        public decimal? tranSettlementAmt { get; set; }
        public DateTime? tranSettlementDt { get; set; }

        public string fxContractRef1 { get; set; }
        public decimal? fxAmountUtilized1 { get; set; }
        public string fxContractRef2 { get; set; }
        public decimal? fxAmountUtilized2 { get; set; }
        public string transactionReference { get; set; }

        public ACK3ResponseSenderParty senderParty { get; set; }
        public ACK3ResponseReceivingParty receivingParty { get; set; }      
    }

    public class ACK3ResponseSenderParty
    {
        public string name { get; set; }
        public string swiftBic { get; set; }
    }

    public class ACK3ResponseReceivingParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
    }
}
