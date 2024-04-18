using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 订单项来源
    /// </summary>
    public enum OrderItemType
    {
        [Description("正常下单")]
        Normal = 1,
        [Description("库房到货")]
        Modified = 2,
    }
}
