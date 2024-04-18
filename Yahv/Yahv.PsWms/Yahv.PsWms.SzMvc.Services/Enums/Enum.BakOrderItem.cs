using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 备份 OrderItem
    /// </summary>
    public enum BakOrderItem
    {
        [Description("topview")]
        DeliveryTopView = 1,

        [Description("origin")]
        OriginOrderItem = 2,

        [Description("new")]
        NewOrderItem = 3,
    }
}
