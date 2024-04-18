using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Enums;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    /// <summary>
    /// 文件
    /// </summary>
    public class PcFile : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 主要ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 文件类型, FileType 枚举
        /// </summary>
        public FileType Type { get; set; }

        /// <summary>
        /// 文件自定义名
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 存储文件的相对路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 网站上传人
        /// </summary>
        public string SiteuserID { get; set; }

        #endregion
    }
}
