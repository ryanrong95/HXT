namespace Yahv.Services.Enums
{
    /// <summary>
    /// 来源(外部事件)
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// 应收(添加费用)
        /// </summary>
        Receivable,

        /// <summary>
        /// 收款确认
        /// </summary>
        Confirm,

        /// <summary>
        /// 优惠券
        /// </summary>
        Coupon,

        /// <summary>
        /// 信用
        /// </summary>
        Credits,

        /// <summary>
        /// 减免
        /// </summary>
        Reduction,

        /// <summary>
        /// 代付收款确认
        /// </summary>
        ConfirmPayFor,

        /// <summary>
        /// 代收收款确认
        /// </summary>
        ConfirmCollecting,

        /// <summary>
        /// 付款确认
        /// </summary>
        ConfirmPayment,
    }
}