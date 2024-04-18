using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace Yahv.Services.Models.WH
{
    public class FileDescription: IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码 四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        public string NoticeID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 客户自定义名称
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type { get; set; }
        /// <summary>
        /// 文件地址(自动变更名称)
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态：200、正常；400、删除；500、停用
        /// </summary>
        public FileStatus Status { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }


        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalFile { get; set; }
        /// <summary>
        /// 文件base64位编码
        /// </summary>
        public string FileBase64Code { get; set; }

        #endregion
    }
}
