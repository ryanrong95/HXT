using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 数据状态
    /// </summary>
    public enum State
    {
        /// <summary>
        /// 在途
        /// </summary>
        [Description("在途")]
        OnTheWay = 0,

        /// <summary>
        /// 揽收
        /// </summary>
        [Description("揽收")]
        Collect = 1,

        /// <summary>
        /// 疑难
        /// </summary>
        [Description("疑难")]
        Difficult = 2,

        /// <summary>
        /// 签收
        /// </summary>
        [Description("签收")]
        SignFor = 3,

        /// <summary>
        /// 退签
        /// </summary>
        [Description("退签")]
        WithdrawalOfSignature = 4,

        /// <summary>
        /// 派件
        /// </summary>
        [Description("派件")]
        Dispatch = 5,

        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        Return = 6,

        /// <summary>
        /// 转投
        /// </summary>
        [Description("转投")]
        Switching = 7,

        /// <summary>
        /// 待清关
        /// </summary>
        [Description("待清关")]
        ToBeCleared = 10,

        /// <summary>
        /// 清关中
        /// </summary>
        [Description("清关中")]
        CustomsClearanceInProgress = 11,

        /// <summary>
        /// 已清关
        /// </summary>
        [Description("已清关")]
        Cleared = 12,

        /// <summary>
        /// 清关异常
        /// </summary>
        [Description("清关异常")]
        AbnormalCustomsClearance = 13,

        /// <summary>
        /// 拒签
        /// </summary>
        [Description("拒签")]
        Refused = 14,
    }
}
