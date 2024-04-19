using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Enums
{
    public enum Status
    {
        [Description("可用")]
        Nomal = 0,
        [Description("不可用")]
        Limited = 1
    }
}
