using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebWeChat.Models
{
    /// <summary>
    /// 付汇记录
    /// </summary>
    public class PayRecordModel
    {
        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplyTime { get; set; } = string.Empty;

        /// <summary>
        /// 用户
        /// </summary>
        public string Applier { get; set; } = string.Empty;

        /// <summary>
        /// 付汇金额
        /// </summary>
        public string Amount { get; set; } = string.Empty;

        /// <summary>
        /// 申请状态
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}