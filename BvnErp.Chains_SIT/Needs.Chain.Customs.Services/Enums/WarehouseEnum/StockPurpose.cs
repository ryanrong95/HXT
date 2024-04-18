using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 出库类型
    /// </summary>
    public enum StockPurpose
    {
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Declared = 1,

        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        Storaged = 2,
    }
}
