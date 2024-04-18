using Yahv.Underly;

namespace Yahv.Services.Models.PvFinance
{
    /// <summary>
    /// 认领收款实体
    /// </summary>
    public class AccountWorksStatistic
    {
        /// <summary>
        /// 实收ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 流水码
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 客户公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 实收
        /// </summary>
        public decimal LeftPrice { get; set; }

        public decimal? RightPrice { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal? Balance => LeftPrice - (RightPrice ?? 0);

        /// <summary>
        /// 认领人ID
        /// </summary>
        public string ClaimantID { get; set; }
    }
}