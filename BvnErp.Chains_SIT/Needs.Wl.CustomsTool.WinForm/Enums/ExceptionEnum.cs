using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Enums
{
    /// <summary>
    /// 异常处理枚举
    /// </summary>
    public enum ExceptionHandlerEnum
    {
        UnTreated = 0,

        RestartCustomsConfig = 1,

        ResendMsgConfig = 2,

        RemindHimConfig = 3,

        OtherExceptionConfig = 4,
    }
}
