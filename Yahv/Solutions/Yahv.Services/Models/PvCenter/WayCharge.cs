using System;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 货物条款
    /// </summary>
    public class WayCharge : IUnique
    {
        public WayCharge()
        {
            this.PayMethod = Methord.TT;
        }

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
        public Currency? Currency { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalPrice { get; set; }
        
        #endregion
    }
}
