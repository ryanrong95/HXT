using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models
{
    /// <summary>
    /// 文件数据模型
    /// </summary>
    public class FileModels
    {
        public string ID { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileFormat { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public Status Status { get; set; }
    }

    /// <summary>
    /// 文件状态
    /// </summary>
    public enum Status
    {
        [Description("上传成功")]
        Success = 0,

        [Description("上传失败")]
        Fail = 1,

        [Description("上传中")]
        UnDo = 2,
    }
}
