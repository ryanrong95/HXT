using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 优惠券类型
    /// </summary>
    public enum CouponType
    {
        /// <summary>
        /// 定额
        /// </summary>
        [Description("抵扣券")]
        Quota = 1,

        /// <summary>
        /// 据实
        /// </summary>
        [Description("体验券")]
        Fact = 2
    }
}
