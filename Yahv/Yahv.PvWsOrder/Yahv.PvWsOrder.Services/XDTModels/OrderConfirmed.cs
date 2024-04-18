using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    /// <summary>
    /// 订单确认
    /// </summary>
    public class OrderConfirmed
    {
        /// <summary>
        /// 主订单-订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        public string CancelReason { get; set; }
    }
}
