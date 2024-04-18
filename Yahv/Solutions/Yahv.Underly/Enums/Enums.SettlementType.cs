using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 流水账类型
    /// </summary>
    public enum SettlementType
    {
        /// <summary>
        /// 约定期限
        /// </summary>
        [Description("约定期限")]
        DueTime = 1,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        Month = 2,
    }
}
