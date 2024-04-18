using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 申请状态
    /// </summary>
    public enum ApplyItemStauts
    {
        /// <summary>
        /// 待支付
        /// </summary>
        [Description("待支付")]
        Paying = 4,

        /// <summary>
        /// 完成支付
        /// </summary>
        [Description("完成支付")]
        Paid = 600,
    }
}
