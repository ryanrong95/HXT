using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvRoute.Services.Enums
{
    /// <summary>
    /// 运单来源
    /// </summary>
    public enum PrintSource
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UKnown =0,
        /// <summary>
        /// 顺丰
        /// </summary>
        [Description("顺丰")]
        SF = 10,
        /// <summary>
        /// 跨越速运
        /// </summary>
        [Description("跨越速运")]
        KYSY = 20
    }
}
