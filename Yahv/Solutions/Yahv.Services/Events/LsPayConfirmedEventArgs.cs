using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services
{
    /// <summary>
    /// 租赁支付确认
    /// </summary>
    public class LsPayConfirmedEventArgs : ConfirmedEventArgs
    {
        public string LsOrderID { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorID { get; set; }
    }
}
