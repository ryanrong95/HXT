using Yahv.Underly.Attributes;

namespace Yahv.XdtData.Import.Enums
{
    /// <summary>
    /// 订单附加费用类型(代理费和商检费 0,1)
    /// </summary>
    public enum OrderPremiumType
    {
        /// <summary>
        /// 代理费
        /// </summary>
        [Description("代理费")]
        AgencyFee,

        /// <summary>
        /// 商检费
        /// </summary>
        [Description("商检费")]
        InspectionFee,

        /// <summary>
        /// 送货费
        /// </summary>
        [Description("送货费")]
        DeliveryFee,

        /// <summary>
        /// 快递费
        /// </summary>
        [Description("快递费")]
        ExpressFee,

        /// <summary>
        /// 清关费
        /// </summary>
        [Description("清关费")]
        CustomClearanceFee,

        /// <summary>
        /// 提货费
        /// </summary>
        [Description("提货费")]
        PickUpFee,

        /// <summary>
        /// 停车费
        /// </summary>
        [Description("停车费")]
        ParkingFee,

        /// <summary>
        /// 入仓费
        /// </summary>
        [Description("入仓费")]
        EntryFee,

        /// <summary>
        /// 仓储费
        /// </summary>
        [Description("仓储费")]
        StorageFee,

        /// <summary>
        /// 收货异常费用
        /// </summary>
        [Description("收货异常费用")]
        UnNormalFee,

        /// <summary>
        /// 其他(的杂费)
        /// </summary>
        [Description("其他")]
        OtherFee
    }
}