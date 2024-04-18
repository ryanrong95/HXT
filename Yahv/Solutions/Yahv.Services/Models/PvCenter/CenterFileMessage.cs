using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 文件消息
    /// </summary>
    public class CenterFileMessage
    {
        public string WsOrderID { get; set; }
        public string LsOrderID { get; set; }
        public string ApplicationID { get; set; }
        public string WaybillID { get; set; }
        public string NoticeID { get; set; }
        public string StorageID { get; set; }
        public string InputID { get; set; }
        public string ClientID { get; set; }
        public string ShipID { get; set; }
        public string PayID { get; set; }
        public string StaffID { get; set; }
        public string ErmApplicationID { get; set; }

        /// <summary>
        /// 上传的原始文件名称
        /// </summary>
        public string CustomName { get; set; }
       
        /// <summary>
        /// 文件类型
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// 文件访问的Url
        /// </summary>
        /// <remarks>
        /// 一定保持唯一
        /// </remarks>
        public string Url { get; set; }
        
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string AdminID { get; set; }
    }
}
