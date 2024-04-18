using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ABESuccResponse
    {
        public ABESuccHeader header { get; set; }
        public AccountBalResponse accountBalResponse { get; set; }       
    }

    public class ABESuccHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public DateTime timeStamp { get; set; }
    }

    public class AccountBalResponse
    {
        public string enqStatus { get; set; }
        public string accountName { get; set; }

        public string accountNo { get; set; }
        public string accountCcy { get; set; }
        public decimal? halfDayHold { get; set; }
        public decimal? oneDayHold { get; set; }
        public decimal? twoDaysHold { get; set; }
        public decimal? clsLedgerBal { get; set; }
        public decimal? clsAvailableBal { get; set; }
        public DateTime businessDate { get; set; }

        public List<AccountBalResponseDtl> accountBalResponseDtl { get; set; }
    }

    public class AccountBalResponseDtl
    {
        public string accountNo { get; set; }
        public string accountCcy { get; set; }
        public decimal? halfDayHold { get; set; }
        public decimal? oneDayHold { get; set; }
        public decimal? twoDaysHold { get; set; }
        public decimal? clsLedgerBal { get; set; }
        public decimal? clsAvailableBal { get; set; }
        public DateTime businessDate { get; set; }
    }
}
