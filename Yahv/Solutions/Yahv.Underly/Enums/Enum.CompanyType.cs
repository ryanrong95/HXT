using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 公司业务类型
    /// </summary>
    public enum CompanyType
    {
        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Purchase = 1,
        /// <summary>
        /// 销售
        /// </summary>
        [Description("销售")]
        Sales = 2,
        /// <summary>
        /// 代购
        /// </summary>
        [Description("代购")]
        CustomBroker = 3,
    }
}
