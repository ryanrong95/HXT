using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum UserStatus
    {
        [Description("停用")]
        Stop = 0,

        [Description("启用")]
        Start = 1,
    }
}
