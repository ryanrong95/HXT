using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 结算方式
    /// </summary>
    public enum QuoteType
    {
        /// <summary>
        /// 阶梯价
        /// </summary>
        [Description("阶梯价")]
        StepPrice = 1,

        /// <summary>
        /// 项目
        /// </summary
        [Description("项目")]
        Project = 2,
       
    }

}
