using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 垫资申请状态
    /// </summary>
    public enum AdvanceMoneyStatus
    {
        [Description("待风控审核")]
        RiskAuditing = 1,

        [Description("待审批")]
        Auditing = 2,

        [Description("已生效")]
        Effective = 3,

        [Description("作废")]
        Delete = 4
    }
    /// <summary>
    /// 订单垫资状态
    /// </summary>
    public enum UntieAdvanceStatus
    {
        [Description("解绑订单垫资")]
        UntieAdvance = 1,
    }
}
