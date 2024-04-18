using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 货运类型
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unkown = 0,

        /// <summary>
        /// 自提
        /// </summary>
        [Description("自提")]
        PickUp = 1,

        /// <summary>
        /// 快递
        /// </summary>
        [Description("快递")]
        Express = 2,

        /// <summary>
        /// 送货上门
        /// </summary>
        [Description("送货上门")]
        Dtd = 3,
    }
}
