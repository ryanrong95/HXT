using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 业务逻辑 枚举
    /// </summary>
    public enum BusinessType
    {
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Declare = 1,

        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        Storage = 2,

        /// <summary>
        /// 消费者
        /// </summary>
        [Description("消费")]
        Customer = 3,
    }
}
