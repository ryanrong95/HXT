using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 自定义产品税号状态
    /// </summary>
    public enum ProductTaxStatus
    {/// <summary>
     /// 待审核
     /// </summary>
        [Description("待审核")]
        Auditing = 100,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 200,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        NotPass = 300,
    }
}
