using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// 我方
        /// </summary>
        [Description("我方")]
        Own = 0,

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

        /// <summary>
        /// 代理线服务
        /// </summary>
        [Description("供应链")]
        Chains = 3,


        /// <summary>
        /// 采购或供应商
        /// </summary>
        [Description("采购")]
        Suppliers = 4

    }


    /// <summary>
    /// 关系类型
    /// </summary>
    public enum RelationType1
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

        /// <summary>
        /// 代理线服务
        /// </summary>
        [Description("供应链")]
        Chains = 3, 
    }
}
