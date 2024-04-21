namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 货款申请
    /// </summary>
    public class PayForGoodsInputDto
    {
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 付款人（公司名称）
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 付款账号
        /// </summary>
        public string PayerCode { get; set; }

        /// <summary>
        /// 付款银行
        /// </summary>
        public string PayerBank { get; set; }

        /// <summary>
        /// 付款账户币种
        /// </summary>
        /// <remarks>Currency的ShortName</remarks>
        public string PayerCurrency { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string PayeeCode { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string PayeeBank { get; set; }

        /// <summary>
        /// 收款币种
        /// </summary>
        public string PayeeCurrency { get; set; }

        /// <summary>
        /// 支付人ID
        /// </summary>
        public string ExecutorID { get; set; }
    }
}