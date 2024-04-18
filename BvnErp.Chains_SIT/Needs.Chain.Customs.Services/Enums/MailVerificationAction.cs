using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{

    /// <summary>
    /// 邮箱验证
    /// </summary>
    public enum MailVerificationAction
    {
        [Description("注册")]
        RA,
        [Description("忘记密码")]
        FPA,
        [Description("更改邮箱")]
        CEA,
    }
}
