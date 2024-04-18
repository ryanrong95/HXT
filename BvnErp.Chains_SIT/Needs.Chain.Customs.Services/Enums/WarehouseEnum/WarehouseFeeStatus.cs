using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 库房费用状态
    /// </summary>
    public enum WarehousePremiumsStatus
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Auditing = 1,

        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Audited = 2,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Payed = 3,
    }
}
