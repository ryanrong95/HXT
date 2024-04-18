using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public class ProductItemFile : Needs.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 凭证类型
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductItemID { get; set; }
        /// <summary>
        /// 子ID
        /// </summary>
        public string SubID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 文件状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public ProductItemFile()
        {
            CreateDate = DateTime.Now;
            Status = Status.Normal;
        }
        #endregion
    }

    /// <summary>
    /// 凭证类型
    /// </summary>
    public enum FileType
    {
        [Description("产品凭证")]
        Item = 100,

        [Description("报备凭证")]
        Report = 200,

        [Description("原厂批复凭证")]
        OriginReply = 300,
    }
}
