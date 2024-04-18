using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Enums
{
    public enum FileStatus
    {
        [Description("正常")]
        Normal = 200,
        [Description("删除")]
        Deleted = 400,
        [Description("停用")]
        StopUsing = 500
    }
}
