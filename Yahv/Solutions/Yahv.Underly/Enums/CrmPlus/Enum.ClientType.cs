using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    /// <summary>
    /// 客户类型
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 终端
        /// </summary>
        [Description("终端")]
        Terminals = 1,

        /// <summary>
        /// 贸易
        /// </summary>
        [Description("贸易")]
        Trader = 2,

        /// <summary>
        /// OEM
        /// </summary>
        [Description("OEM")]
        OEM = 3,
        [Description("ODM")]
        ODM=4,

        [Description("高校级科研院所")]
        University = 5,
    }
}
