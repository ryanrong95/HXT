using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    [Flags]
    public enum JobType
    {
        [Description("销售")]
        Sales = 100,

        [Description("技术")]
        FAE = 200,

        [Description("产品")]
        PME = 400,

        [Description("销售/产品")]
        Sales_PME = 500,

        [Description("总PM")]
        TPM = 800
    }

    [Flags]
    public enum ScoreType
    {
        [Description("销售")]
        Sales = 100,

        [Description("技术")]
        FAE = 200,

        [Description("产品")]
        PME = 400,

        [Description("不考核")]
        Not = 0,
    }
}
