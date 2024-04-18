using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FXBookingRequest
    {
        public FXQuoteHeader header { get; set; }
        public FXBookingTxnInfo txnInfo { get; set; }
    }

    public class FXBookingTxnInfo
    {
        public string uid { get; set; }
        public string clientTxnsId { get; set; }       
    }
}
