using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public enum Business
    {

        /// <summary>
        /// 传统贸易
        /// </summary>

        [Description("传统贸易")]
        Trading = 10,

        /// <summary>
        /// 报关服务
        /// </summary>
        [Description("报关服务")]
        CustombrokerServicing = 20,
        /// <summary>
        /// 代仓储
        /// </summary>
        [Description("代仓储")]
        WarehouseServicing = 30,

        /// <summary>
        /// 代理线服务
        /// </summary>
        [Description("代理线服务")]
        AagentServicing = 40,
        /// <summary>
        /// 传统贸易-销售
        /// </summary>

        [Description("传统贸易-销售")]
        Trading_Sale = 101,
        /// <summary>
        /// 传统贸易-销售
        /// </summary>

        [Description("传统贸易-采购")]
        Trading_Purchase = 102

    }
}
