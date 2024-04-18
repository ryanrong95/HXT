using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ARERequest
    {
        public AREHeader header { get; set; }
        public AREAccInfo accInfo { get; set; }
    }

    public class AREHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string timeStamp { get; set; }
        public string ctry { get; set; }
    }

    public class AREAccInfo
    {
        public string accountNo { get; set; }
        public string accountCcy { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
