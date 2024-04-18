using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum CollectStatusEnums
    {
        /// <summary>
        /// 未收款
        /// </summary>
        [Description("未收款")]
        UnCollected,

        /// <summary>
        /// 部分收款
        /// </summary>
        [Description("部分收款")]
        PartCollected,

        /// <summary>
        /// 已收款
        /// </summary>
        [Description("已收款")]
        Collected
    }

    public enum MatchStatusEnums
    {
        /// <summary>
        /// 未匹配
        /// </summary>
        [Description("未匹配")]
        UnMatched,

        /// <summary>
        /// 已匹配
        /// </summary>
        [Description("已匹配")]
        Matched
    }

    public enum ItemCollectStatusEnums
    {
        /// <summary>
        /// 付汇申请项未匹配
        /// </summary>
        [Description("未匹配")]
        UnMatched,

        /// <summary>
        /// 付汇申请项已匹配
        /// </summary>
        [Description("已匹配")]
        Matched
    }
}
