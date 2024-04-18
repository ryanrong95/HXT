using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ProjectType
    {
        [Description("代理")]
        Agent = 10,

        [Description("非代理")]
        Normal = 20,

    }
}
