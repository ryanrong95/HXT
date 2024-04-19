using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum PayMethod
    {
        [Description("信用证")]
        LC = 0,
        [Description("电汇")]
        TT = 1,
        [Description("付款交单")]
        DP = 2
    }
}
