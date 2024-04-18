using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 提货通知状态
    /// </summary>
    public enum DeliveryOperType
    {
        [Description("新增提货通知")]
        AddDelivery = 1,

        [Description("打印提货单")]
        PrintDelivery = 2,

        [Description("提货完成")]
        HadDelivered = 3
    }
}
