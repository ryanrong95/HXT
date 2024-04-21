namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 核销
    /// </summary>
    public class PayeeRightInputDto
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 核销金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountNo { get; set; }
    }
}