using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.Csrm.Services.Models
{
    /// <summary>
    /// 文件消息
    /// </summary>
    public class FileMessage
    {
        public string WsOrderID { get; set; }

        public string LsOrderID { get; set; }
        public string ApplicationID { get; set; }
        public string WaybillID { get; set; }
        public string NoticeID { get; set; }
        public string StorageID { get; set; }
        public string InputID { get; set; }
        public string ClientID { get; set; }

        /// <summary>
        /// 支付ID
        /// </summary>
        /// <remarks>
        /// 应收
        /// 应付
        /// </remarks>
        public string PayID { get; set; }

        public string StaffID { get; set; }
        public string ErmApplicationID { get; set; }

        /// <summary>
        /// 文件的客户命名
        /// </summary>
        public string CustomName { get; set; }

        public int Type { get; set; }
        /// <summary>
        /// 文件访问的Url
        /// </summary>
        /// <remarks>
        /// 一定保持唯一
        /// </remarks>
        public string Url { get; set; }

        public string AdminID { get; set; }
        public string ShipID { get; set; }

        ///// <summary>
        ///// 发票ID
        ///// </summary>
        //public string InvoiceCode { get; set; }

    }
}
