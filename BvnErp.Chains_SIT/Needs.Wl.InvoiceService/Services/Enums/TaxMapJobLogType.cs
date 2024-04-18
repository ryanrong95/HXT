using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Enums
{
    public enum TaxMapJobLogType
    {
        /// <summary>
        /// Job正常进入
        /// </summary>
        [Description("Job正常进入")]
        JobNormalEntry = 1,

        /// <summary>
        /// Job正常退出
        /// </summary>
        [Description("Job正常退出")]
        JobNormalExit = 2,

        /// <summary>
        /// Job中有异常
        /// </summary>
        [Description("Job中有异常")]
        JobException = 3,
    }
}
