using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    ///申请状态CrmPlus
    /// </summary>
    public enum ApplyStatus
    {

        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Waiting = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("通过")]
        Allowed = 200,
        /// <summary>
        /// 否决
        /// </summary>
        [Description("否决")]
        Voted = 300,
        /// <summary>
        /// 撤销申请
        /// </summary>
        [Description("撤销申请")]
        Cancel = 400,
    }

}
