using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    /// <summary>
    /// 优惠券有效期类型
    /// </summary>
    public enum CouponValidType
    {

        [Description("固定时间")]
        FixedTime = 1,

        [Description("固定时长")]
        FixedDuration = 2
    }

}
