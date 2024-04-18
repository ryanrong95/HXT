using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 客户性质
    /// </summary>
    public enum ClientType
    {

        ///// <summary>
        ///// 全部
        ///// </summary>
        //[Description("全部")]
        //All = short.MinValue,

        /// <summary>
        /// 终端
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



    }
}
