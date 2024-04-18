
namespace Needs.Ccs.Services.Models
{
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
        public string PayID { get; set; }

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
        /// <summary>
        /// 运输批次ID
        /// </summary>
        public string ShipID { get; set; }
    }
}
