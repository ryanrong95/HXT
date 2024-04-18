using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 出库通知状态
    /// </summary>
    public enum ExitNoticeStatus
    {
        /// <summary>
        /// 未出库
        /// </summary>
        [Description("未出库")]
        UnExited = 1,

        /// <summary>
        /// 待出库
        /// </summary>
        [Description("待出库")]
        Exiting = 2,

        /// <summary>
        /// 已出库
        /// </summary>
        [Description("已出库")]
        Exited = 4,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 5,
    }
}
