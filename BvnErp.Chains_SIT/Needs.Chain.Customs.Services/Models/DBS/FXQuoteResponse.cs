using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FXQuoteResponse
    {
        public FXQuoteHeader header { get; set; }
        public FXQuoteTxnResponse txnResponse { get; set; }
    }

   
    public class FXQuoteTxnResponse
    {
        public string txnStatus { get; set; }
        public string uid { get; set; }
        public string validTill { get; set; }
        public decimal rate { get; set; }
        public string ccyPair { get; set; }
        public string dealtSide { get; set; }
        public string tenor { get; set; }
        public string txnCcy { get; set; }
        public string valueDate { get; set; }
        public decimal txnAmount { get; set; }
        public string clientTxnsId { get; set; }
        public string contraCcy { get; set; }
        public decimal contraAmount { get; set; }
        public string dealType { get; set; }
        public string traceId { get; set; }
        public string txnRejectCode { get; set; }
        public string txnStatusDescription { get; set; }
    }
}
