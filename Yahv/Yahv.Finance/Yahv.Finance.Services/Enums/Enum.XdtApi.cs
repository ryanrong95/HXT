using Yahv.Underly.Attributes;

namespace Yahv.Finance.Services
{
    /// <summary>
    /// 芯达通Api
    /// </summary>
    public enum XdtApi
    {
        /// <summary>
        /// 金库新增、修改
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/CenterVaultUpdate")]
        GoldStoreEnter,

        /// <summary>
        /// 账户新增、修改
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/CenterAccountUpdate")]
        AccountEnter,

        /// <summary>
        /// 收款操作
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/FinanceReceipt")]
        PayeeLeftEnter,

        /// <summary>
        /// 付款操作
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/FinancePayment")]
        PayerEnter,

        /// <summary>
        /// 费用操作
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/FinanceFee")]
        FinanceFee,

        /// <summary>
        /// 资金调拨
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/FundTransfer")]
        FundTransfer,

        /// <summary>
        /// 批量收款
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/FinanceReceiptMulti")]
        PayeeLeftEnterBatch,

        /// <summary>
        /// 新增承兑汇票
        /// </summary>
        [ApiAddress("XdtApiHostName", "/Finance/AcceptanceBill")]
        AcceptanceBill,
    }
}