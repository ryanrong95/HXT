using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.XdtData.Import.Enums
{
    /// <summary>
    /// 芯达通截单状态
    /// </summary>
    public enum CutStatus
    {
        /// <summary>
        /// 未截单
        /// </summary>
        [Description("未截单")]
        UnCutting = 0,

        /// <summary>
        /// 已截单
        /// </summary>
        [Description("已截单")]
        Cutted = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed = 2
    }
}
