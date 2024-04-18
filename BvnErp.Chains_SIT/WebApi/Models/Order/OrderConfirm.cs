using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.Order
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

    /// <summary>
    /// 用户确定订单收货接收Model
    /// </summary>
    public class UserConfirmReceiptModel
    {
        /// <summary>
        /// 主订单号
        /// </summary>
        public string MainOrderID { get; set; } = string.Empty;
    }

}