using Needs.Linq;


namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 货物条款
    /// </summary>
    public class WayCharge : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 代付货款、代收货款、忽略
        /// </summary>
        public WayChargeType? Payer { get; set; }

        /// <summary>
        /// 货款支付/收取方式 现金 支票  转账
        /// </summary>
        public Methord? PayMethod { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Needs.Ccs.Services.Models.ApiModels.Warehouse.Currency? Currency { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalPrice { get; set; }

        #endregion
    }
}
