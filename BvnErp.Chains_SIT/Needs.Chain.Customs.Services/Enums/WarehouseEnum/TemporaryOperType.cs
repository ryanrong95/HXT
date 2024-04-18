using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum TemporaryOperType
    {
        [Description("新增")]
        Created = 1,

        [Description("暂存通知")]
        TemporaryNotice = 2,

        [Description("已暂存")]
        Temporaryed = 3,

        [Description("已下单")]
        Ordered = 4,

        [Description("完成装箱")]
        Complete = 5,
    }
}
