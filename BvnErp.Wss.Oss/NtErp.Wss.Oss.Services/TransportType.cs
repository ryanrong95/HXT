using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 条款类型
    /// </summary>
    public enum TransportType
    {
        [Description("港-港", "常用的运输条款")]
        CY_CY,
        [Description("港-门", "要有收货人指定的详细地址", "报关的启始")]
        CY_DR,
        [Description("门-门", "要有收货人指定的详细地址")]
        DR_DR
    }
}
