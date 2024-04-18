using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 应付实付
    /// </summary>
    public class PaymentStatistic : Yahv.Services.Models.PaymentsStatistic, IUnique
    {
        public string ID { get; set; }

        #region 扩展属性
        /// <summary>
        /// 收款公司名称
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 付款公司名称
        /// </summary>
        public string PayerName { get; set; }
        #endregion
    }
}
