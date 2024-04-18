using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ABERequest
    {
        public ABEHeader header { get; set; }
        public ABETxnInfo txnInfo { get; set; }
        public ABEAccountBalInfo accountBalInfo { get; set; }
    }

    public class ABEHeader
    {
        public string msgId { get; set; }
        public string orgId { get; set; }
        public string timeStamp { get; set; }
        public string ctry { get; set; }
    }

    public class ABETxnInfo
    {
        /// <summary>
        /// 查询余额，值为BLE
        /// </summary>
        public string txnType { get; set; }
    }

    public class ABEAccountBalInfo
    {
        public string accountNo { get; set; }
       
    }
}
