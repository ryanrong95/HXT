using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 归类状态
    /// </summary>
    public enum ClassifyStatus
    {
        /// <summary>
        /// 未归类
        /// </summary>
        [Description("未归类")]
        Unclassified,

        /// <summary>
        /// 首次归类
        /// </summary>
        [Description("首次归类")]
        First,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        Anomaly,

        /// <summary>
        /// 归类完成
        /// </summary>
        [Description("归类完成")]
        Done
    }

    /// <summary>
    /// 归类类型
    /// </summary>
    public enum ClassifyType
    {
        [Description("产品归类")]
        ProductClassify = 1,

        [Description("产品预归类")]
        ProductPreClassify = 2
    }

    /// <summary>
    /// 归类阶段
    /// </summary>
    public enum ClassifyStep
    {
        /// <summary>
        /// 产品归类预处理一
        /// </summary>
        [Description("预处理一")]
        Step1 = 1,

        /// <summary>
        /// 产品归类预处理二
        /// </summary>
        [Description("预处理二")]
        Step2 = 2,

        /// <summary>
        /// 产品归类完成
        /// </summary>
        [Description("已完成")]
        Done = 3,

        /// <summary>
        /// 产品变更/重新归类
        /// </summary>
        [Description("重新归类")]
        ReClassify = 4,

        /// <summary>
        /// 重新归类完成
        /// </summary>
        [Description("重新归类完成")]
        ReClassified = 5,

        /// <summary>
        /// 产品预归类预处理一
        /// </summary>
        [Description("预处理一")]
        PreStep1 = 6,

        /// <summary>
        /// 产品预归类预处理二
        /// </summary>
        [Description("预处理二")]
        PreStep2 = 7,

        /// <summary>
        /// 产品预归类完成
        /// </summary>
        [Description("已完成")]
        PreDone = 8,

        /// <summary>
        /// 已完成中归类
        /// </summary>
        [Description("已完成中归类")]
        DoneEdit = 9,

        /// <summary>
        /// 预归类已完成中归类
        /// </summary>
        [Description("预归类已完成中归类")]
        PreDoneEdit = 10,
    }

    /// <summary>
    /// 报关员角色
    /// </summary>
    public enum DeclarantRole
    {
        [Description("初级报关员")]
        JuniorDeclarant = 1,

        [Description("高级报关员")]
        SeniorDeclarant = 2
    }
}
