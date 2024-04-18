using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FXQuoteRequest
    {
        public FXQuoteHeader header { get; set; }
        public FXQuoteTxnInfo txnInfo { get; set; }
    }

    public class FXQuoteHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string timeStamp { get; set; }      
    }

    public class FXQuoteTxnInfo
    {        
        public string ccyPair { get; set; }
        public string dealtSide { get; set; }
        public decimal txnAmount { get; set; }
        public string txnCcy { get; set; }
        public string tenor { get; set; }
        public string clientTxnsId { get; set; }
    }
}
