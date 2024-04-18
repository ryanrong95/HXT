using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 承运商类型
    /// </summary>
    public enum CarrierType
    {

        /// <summary>
        /// 物流
        /// </summary>
        [Description("物流")]
        Logistics = 1,
        /// <summary>
        /// 快递
        /// </summary>
        [Description("快递")]
        Express = 2,

        /// <summary>
        /// 物流快递
        /// </summary>
        [Description("物流快递")]
        Both = 3
    }
}
