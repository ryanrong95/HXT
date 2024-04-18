using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 通知来源
    /// </summary>
    public enum NoticeSource
    {
        /// <summary>
        /// 库房
        /// </summary>
        [Description("库房")]
        Keeper = 1,

        /// <summary>
        /// 跟单
        /// </summary>
        [Description("跟单")]
        Tracker = 2,
    }
}
