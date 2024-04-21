using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvRoute.Services.Enums
{
    /// <summary>
    /// 运单状态
    /// </summary>
    public enum PrintState
    {
        /// <summary>
        /// 正常的
        /// </summary>
        [Description("正常的")]
        Normal = 10,
        /// <summary>
        /// 无效的
        /// </summary>
        [Description("无效的")]
        Waiting = 20
    }
}
