using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Enums
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Auditing = 100,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 400
    }
}
