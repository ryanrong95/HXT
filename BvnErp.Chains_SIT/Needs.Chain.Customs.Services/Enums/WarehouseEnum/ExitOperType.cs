using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum ExitOperType
    {
        [Description("通知")]
        Noticed = 1,

        [Description("打印")]
        Printed = 2,

        [Description("出库")]
        Exited = 3,

        [Description("送货完成")]
        Completed = 4
    }
}
