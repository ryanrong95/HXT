using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace Yahv.Services
{
    /// <summary>
    /// 仓储支付确认
    /// </summary>
    public class WhsPayConfirmedEventArgs : ConfirmedEventArgs
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public SourceType Source { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderPaymentStatus Status { get; set; }
    }
}
