using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 邮箱验证状态
    /// </summary>
    public enum MailVerificationStatus
    {
        [Description("未验证")]
        Unverified = 10,

        [Description("已验证")]
        Verified = 20,
        [Description("失效")]
        Invalid = 20,
    }
}
