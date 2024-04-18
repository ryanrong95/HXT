using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 资金调拨审批状态
    /// </summary>
    public enum  FundTransferApplyStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Approving = 1,

        /// <summary>
        /// 待付款
        /// </summary>
        [Description("待付款")]
        Paying = 2,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Done = 3,

        /// <summary>
        /// 拒绝
        /// </summary>
        [Description("拒绝")]
        Denied = 4,
    }
}
