using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.Csrm.Services
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,
        ///  /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Closed = 500,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 600,

    }
}
