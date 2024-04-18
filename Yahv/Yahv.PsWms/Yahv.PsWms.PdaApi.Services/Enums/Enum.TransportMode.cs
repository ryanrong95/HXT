using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 货运类型
    /// </summary>
    public enum TransportMode
    {
        [Description("未知")]
        Unkown = 0,

        [Description("自提")]
        PickUp = 1,

        [Description("快递")]
        Express = 2,

        [Description("送货上门")]
        Dtd = 3
    }
}
