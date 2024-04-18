
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 商务公司
    /// </summary>
    public enum BusinessRelationType
    {
        /// <summary>
        /// 代理公司
        /// </summary>
        [Description("代理公司")]
        Agent = 1,
        /// <summary>
        /// 客户公司
        /// </summary>
        [Description("客户公司")]
        Client = 2,
        /// <summary>
        /// 下单公司
        /// </summary>
        [Description("下单公司")]
        Place = 3,
        /// <summary>
        /// 设计公司
        /// </summary>
        [Description("设计公司")]
        Designer = 4,

        /// <summary>
        /// 兄弟关系
        /// </summary>
        [Description("兄弟关系")]
        Brother = 5,

        /// <summary>
        /// 母子关系
        /// </summary>
        [Description("母子关系")]
        Depend = 6,    
    }

    /// <summary>
    /// Site商务公司
    /// </summary>
    public enum SiteBusinessRelationType
    {       
        /// <summary>
        /// 集团公司
        /// </summary>
        [Description("集团公司")]
        Group = 7,

        /// <summary>
        /// 分公司
        /// </summary>
        [Description("分公司")]
        Branch = 8,
        /// <summary>
        /// 商务
        /// </summary>
        [Description("商务")]
        Business = 9
    }

}
