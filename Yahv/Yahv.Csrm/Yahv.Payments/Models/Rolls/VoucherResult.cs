namespace Yahv.Payments.Models.Rolls
{
    /// <summary>
    /// 核销返回结果
    /// </summary>
    public class VoucherResult
    {
        /// <summary>
        /// 结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 核销金额
        /// </summary>
        public decimal WriteOffPrice { get; set; }
    }
}