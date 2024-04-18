using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    /// <summary>
    /// 企业类型
    /// </summary>
    public enum EnterpriseType
    {
        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Supplier = 1,
        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Client = 2,
        /// <summary>
        /// 公司
        /// </summary>
        [Description("公司")]
        Company = 3,
        /// <summary>
        /// 货代
        /// </summary>
        [Description("货代")]
        CustomBroker = 4,
        /// <summary>
        /// 代仓储供应商
        /// </summary>
        [Description("代仓储供应商")]
        WsSupplier = 5,
        /// <summary>
        /// 代仓储客户
        /// </summary>
        [Description("代仓储客户")]
        WsClient = 7,
        /// <summary>
        /// 库房
        /// </summary>
        [Description("库房")]
        WareHouse = 8,
    }
}
