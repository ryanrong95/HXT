using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum AgreementChangeApplyStatus
    {
        [Description("待审核")]
        RiskAuditing = 1,

        [Description("待审批")]
        Auditing = 2,

        [Description("已生效")]
        Effective = 3,

        [Description("作废")]
        Delete = 4
    }
}