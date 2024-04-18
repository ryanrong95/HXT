using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 报关单（换汇通知）换汇状态
    /// </summary>
    public enum SwapStatus
    {
        /// <summary>
        /// 未换汇
        /// </summary>
        [Description("未换汇")]
        UnAuditing = 0,

        /// <summary>
        /// 待换汇
        /// </summary>
        [Description("待换汇")]
        Auditing = 1,

        /// <summary>
        /// 已换汇
        /// </summary>
        [Description("已换汇")]
        Audited = 2,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 3,

        /// <summary>
        /// 部分换汇
        /// </summary>
        [Description("部分换汇")]
        PartAudit = 4,

        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        ApprovedAudit = 5,

        /// <summary>
        /// 审批不通过
        /// </summary>
        [Description("审批不通过")]
        RefuseAudit = 6,
    }

    /// <summary>
    /// SwapNotice 中 IsReceiptUseOut 字段状态
    /// </summary>
    public enum ReceiptUseOutType
    {
        /// <summary>
        /// 没使用或未用完
        /// </summary>
        [Description("没使用或未用完")]
        NoOut = 0,

        /// <summary>
        /// 已用完
        /// </summary>
        [Description("已用完")]
        Out = 1,
    }
}
