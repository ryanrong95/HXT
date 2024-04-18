using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 自定义产品税号状态
    /// </summary>
    public enum ProductTaxStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Auditing = 100,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 200,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        NotPass = 300,
    }
}
