using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 入库通知状态
    /// </summary>
    public enum PackingStatus
    {
        [Description("未封箱")]
        UnSealed = 1,
        [Description("已封箱")]
        Sealed = 2,
        [Description("已出库")]
        Exited = 3
    }
}
