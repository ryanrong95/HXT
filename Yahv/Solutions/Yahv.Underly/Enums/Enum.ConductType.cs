using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 理想中的业务分类：代理线是贸易的一部分
    /// </summary>
    public enum IdealConduct {
        /// <summary>
        /// 贸易
        /// </summary>
        [Description("贸易")]
        Trade = 1,
        /// <summary>
        /// 供应链
        /// </summary>
        [Description("供应链")]
        Chains =2
    }
    /// <summary>
    /// 业务类型
    /// </summary>
    public enum ConductType
    {
        /// <summary>
        /// 贸易
        /// </summary>
        [Description("贸易")]
        Trade = 1,

        /// <summary>
        /// 代理线
        /// </summary>
        [Description("代理线")]
        AgentLine = 2,

        ///// <summary>
        ///// 代理线
        ///// </summary>
        //[Description("供应链")]
        //Chains = 2,

        ///// <summary>
        ///// 代理线
        ///// </summary>
        //[Description("代理线")]
        //PostSale = 3
    }

}
