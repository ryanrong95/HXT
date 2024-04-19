using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 运输方式
    /// </summary>
    public enum TransportMode
    {
        [Description("水路运输")]
        Waterage,
        [Description("铁路运输")]
        Railage,
        [Description("航空运输")]
        Airservice,
        [Description("公路运输")]
        Haulage,
        [Description("顺丰")]
        Shunfeng,
        [Description("FedEx")]
        FedEx,
        [Description("Ups")]
        Ups,
        [Description("客户自提")]
        CustomPick,
        [Description("Ups")]
        DHL,
        [Description("Other")]
        Other
    }
}
