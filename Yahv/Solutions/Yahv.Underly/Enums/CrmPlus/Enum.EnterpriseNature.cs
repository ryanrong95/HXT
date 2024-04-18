using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 企业性质
    /// </summary>
    public enum EnterpriseNature
    {
        /// <summary>
        /// 国有企业
        /// </summary>
        [Description("国有企业")]
        StateOwned = 1,
        /// <summary>
        /// 国有控股
        /// </summary>
        [Description("国有控股")]
        StateHolding = 2,
        /// <summary>
        /// 外资企业
        /// </summary>
        [Description("外资企业")]
        ForeignFunded = 3,
        /// <summary>
        /// 合资企业
        /// </summary>
        [Description("合资企业")]
        JointVentures = 4,
        /// <summary>
        /// 私营企业
        /// </summary>
        [Description("私营企业")]
        PrivateEnterprises = 5,
        /// <summary>
        /// 上市公司
        /// </summary>
        [Description("上市公司")]
        listed = 6,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 99
    }
}
