using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 华芯通服务类型
    /// </summary>
    [Flags]
    public enum ServiceType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Customs = 1,

        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        Warehouse = 2,

        /// <summary>
        /// 双服务
        /// </summary>
        [Description("双服务")]
        Both = 3
    }

    /// <summary>
    /// 客户身份
    /// </summary>
    /// <remarks>
    /// 理论上不合理
    /// </remarks>
    public enum WsIdentity
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("香港")]
        HongKong = 1,
        /// <summary>
        /// 终端
        /// </summary>
        [Description("大陆")]
        Mainland = 2,

        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("个人")]
        Personal = 3,
    }
}
