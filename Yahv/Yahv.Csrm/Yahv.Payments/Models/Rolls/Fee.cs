namespace Yahv.Payments.Models
{
    /// <summary>
    /// 核销金额
    /// </summary>
    public class Fee
    {
        /// <summary>
        /// 应收ID
        /// </summary>
        public string LeftID { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? RightPrice { get; set; }
    }
}