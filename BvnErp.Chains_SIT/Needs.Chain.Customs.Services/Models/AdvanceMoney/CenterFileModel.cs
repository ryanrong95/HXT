using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterFileModel : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        public string WsOrderID { get; set; }
        public string LsOrderID { get; set; }
        public string ApplicationID { get; set; }
        public string WaybillID { get; set; }
        public string NoticeID { get; set; }
        public string StorageID { get; set; }
        public string InputID { get; set; }
        public string ClientID { get; set; }
        public string PayID { get; set; }

        //public string ID { get; set; }
        //public string ApplyID { get; set; }
        public string AdminID { get; set; }
        public string UserID { get; set; }
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public string FileFormat { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }

        /// <summary>
        /// 上传的原始文件名称
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type { get; set; }
    }
}
