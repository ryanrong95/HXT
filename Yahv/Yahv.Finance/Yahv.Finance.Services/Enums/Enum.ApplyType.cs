namespace Yahv.Finance.Services.Enums
{
    /// <summary>
    /// 申请类型
    /// </summary>
    public enum ApplyType
    {
        /// <summary>
        /// 货款申请
        /// </summary>
        ProductsFee = 10,
        /// <summary>
        /// 资金申请
        /// </summary>
        CostApply = 20,
        /// <summary>
        /// 资金调拨申请
        /// </summary>
        FundTransfer = 30,
        /// <summary>
        /// 费用申请
        /// </summary>
        ChargeApply = 40,
        /// <summary>
        /// 预收退款申请
        /// </summary>
        RefundApply = 50,
        /// <summary>
        /// 承兑调拨申请
        /// </summary>
        AcceptanceApply = 60,
    }
}