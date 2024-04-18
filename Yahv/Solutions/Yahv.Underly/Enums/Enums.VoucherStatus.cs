using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 账单状态
    /// </summary>
    public enum VoucherStatus
    {
        /// <summary>
        /// 待确认
        /// </summary>
        [Description("待确认")]
        Save = 100,

        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认")]
        Confirmed = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("关闭")]
        Deleted = 400
    }
}
