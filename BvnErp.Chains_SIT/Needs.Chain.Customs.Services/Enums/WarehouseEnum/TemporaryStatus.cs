using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum TemporaryStatus
    {
        [Description("未处理")]
        Untreated = 1,

        [Description("已处理")]
        Treated = 2,

        [Description("已完成")]
        Complete = 3,
    }
}
