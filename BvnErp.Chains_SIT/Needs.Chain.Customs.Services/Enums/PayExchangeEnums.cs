using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 付汇申请状态
    /// </summary>
    public enum PayExchangeApplyStatus
    {
        /// <summary>
        /// 状态：已申请（待审核）、已审核、已取消
        /// </summary>
        [Description("待审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,

        /// <summary>
        /// 已审批
        /// </summary>
        [Description("已审批")]
        Approvaled = 3,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancled = 4,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed= 5
    }

    /// <summary>
    /// 已换汇申请处理状态
    /// </summary>
    public enum SwapedNoticeHandleStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        UnHandle = 1,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Handled = 2,
    }

    /// <summary>
    /// 区域类型
    /// </summary>
    public enum RegionalType
    {

        /// <summary>
        /// 美国
        /// </summary>
        [Description("美国")]
        ABA = 1,
        /// <summary>
        /// 欧盟
        /// </summary>
        [Description("欧盟")]
        IBAN = 2,


    }

    /// <summary>
    /// 代付款手续费类型
    /// </summary>
    public enum HandlingFeePayerType
    {
        [Description("收款方")]
        收款方 = 1,

        [Description("付款方")]
        付款方 = 2,

        [Description("双方承担")]
        双方承担 = 3,
    }
}