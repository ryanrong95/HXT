using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Wss.Sales.Services.Utils.Structures;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum SelfStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Naming("待审核")]
        Auditing = 100,
        /// <summary>
        /// 已否决
        /// </summary>
        [Naming("已否决")]
        Vetoed = 104,
        /// <summary>
        /// 正常
        /// </summary>
        [Naming("正常")]
        Normal = 200,
        /// <summary>
        /// 已删除
        /// </summary>
        [Naming("已删除")]
        Deleted = 400,

        [Naming("超级数据")]
        Super = 10000
    }
}
