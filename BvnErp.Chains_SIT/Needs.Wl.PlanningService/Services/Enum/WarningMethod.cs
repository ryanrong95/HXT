using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public enum WarningMethod
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        Mail = 1,

        /// <summary>
        /// 发短信
        /// </summary>
        Msg = 2,
    }
}
