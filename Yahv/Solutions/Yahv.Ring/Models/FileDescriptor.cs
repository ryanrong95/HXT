using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Models
{
    /// <summary>
    /// 这些应该开发在逻辑层中,文件类型
    /// </summary>
    /// <remarks>乔霞前端传递给 PC</remarks>
    public class FileDescriptor
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 客户命名
        /// </summary>
        public int CustomName { get; set; }

        /// <summary>
        /// 文件保存的Url地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateDate { get; set; }

        /// <summary>
        /// 客户上传的就直接为客户名称
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 管理员上传的就直接为客户名称
        /// </summary>
        public int AdminID { get; set; }
    }
}
