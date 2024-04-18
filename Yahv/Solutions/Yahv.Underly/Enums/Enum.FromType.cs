using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 客户、供应商子对象来源
    /// </summary>
    public enum FromType
    {
        /// <summary>
        /// 传统贸易
        /// </summary>
        [Description("传统贸易")]
        Trade = 1,
        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        WarehouseServicing = 2,

        /// <summary>
        /// 报关服务
        /// </summary>
        [Description("报关服务")]
        CustomsServicing = 3,
        /// <summary>
        /// 内部公司
        /// </summary>
        [Description("内部公司")]
        InternalCompany = 4,
        /// <summary>
        /// 内部公司
        /// </summary>
        [Description("代仓储客户")]
        WsClient = 5,
    }
}
