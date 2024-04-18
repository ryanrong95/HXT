using Needs.Utils.Descriptions;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 内单
        /// </summary>
        [Description("A类")]
        Inside = 100,

        /// <summary>
        /// 外单
        /// </summary>
        [Description("B类")]
        Outside = 200,

        /// <summary>
        /// Icgoo
        /// </summary>
        [Description("Icgoo")]
        Icgoo = 300
    }

    /// <summary>
    /// 香港交货方式
    /// </summary>
    public enum HKDeliveryType
    {
        [Description("送货")]
        SentToHKWarehouse = 1,

        [Description("自提")]
        PickUp = 2
    }

    /// <summary>
    /// 深圳交货方式 
    /// </summary>
    public enum SZDeliveryType
    {
        [Description("自提")]
        PickUpInStore = 1,

        [Description("送货")]
        SentToClient = 2,

        [Description("代发货")]
        Shipping = 3
    }

    /// <summary>
    /// 订单特殊类型
    /// </summary>
    public enum OrderSpecialType
    {
        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 1,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 2,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 3,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 4,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 5,
    }
}
