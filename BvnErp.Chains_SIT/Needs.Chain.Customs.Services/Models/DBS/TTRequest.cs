using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TTRequest
    {
        public ACTRequestHeader header { get; set; }

        public TTRequestTxnInfoDetails txnInfoDetails { get; set; }
    }

    public class TTRequestTxnInfoDetails
    {
        public List<TTRequestTxnInfo> txnInfo { get; set; }
    }

    public class TTRequestTxnInfo
    {
        public string customerReference { get; set; }
        public string txnType { get; set; }
        public string txnDate { get; set; }
        public string txnCcy { get; set; }
        public decimal txnAmount { get; set; }
        public string debitAccountCcy { get; set; }
        public string fxContractRef1 { get; set; }
        public decimal fxAmountUtilized1 { get; set; }
        public string chargeBearer { get; set; }
        public CNAPSSenderParty senderParty { get; set; }
        public TTReceivingParty receivingParty { get; set; }
        public TTBopInfo bopInfo { get; set; }

        //public List<TTDocuments> documents { get; set; }
    }

    public class TTReceivingParty
    {
        public string name { get; set; }
        public string accountNo { get; set; }
        public string bankCtryCode { get; set; }
        public string swiftBic { get; set; }
        public string bankName { get; set; }
        public List<TTBeneficiaryAddresses> beneficiaryAddresses { get; set; }        
    }

    public class TTBeneficiaryAddresses
    {
        public string address { get; set; }
    }

    public class TTBopInfo
    {
        public string specificPaymentPurpose { get; set; }
        public string taxFreeGoodsRelated { get; set; }
        public string paymentNature { get; set; }
        public string bOPCode1PaymentCategory { get; set; }
        public string bOPCode1SeriesCode { get; set; }
        public string transactionRemarks1 { get; set; }
        public string counterPartyCtryCode { get; set; }
    }

    public class TTDocuments
    {
        public string documentName { get; set; }
        public string encodedFile { get; set; }
    }

}
