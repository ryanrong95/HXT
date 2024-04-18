using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 待发货的发货类型
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>
        /// 本港发货
        /// </summary>
        [Description("本港发货")]
        HKShipment = 1,

        /// <summary>
        /// 国际发货
        /// </summary>
        [Description("国际发货")]
        InternationalShipment = 2,
    }
}
