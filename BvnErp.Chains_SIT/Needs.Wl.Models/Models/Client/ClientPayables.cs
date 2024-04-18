using Needs.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户应付款统计
    /// </summary>
    public class ClientPayables : IUnique
    {
        /// <summary>
        ///  ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 应付货款
        /// </summary>
        public decimal? ProductPayable { get; set; }

        /// <summary>
        /// 应付代理费
        /// </summary>
        public decimal? AgencyFeePayable { get; set; }

        /// <summary>
        /// 应付杂费
        /// </summary>
        public decimal? TaxPayable { get; set; }

        /// <summary>
        /// 应付杂费 
        /// </summary>
        public decimal? IncidentalPayable { get; set; }
    }
}