using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class WsOrderOutput : Yahv.Linq.IUnique
    {
        #region 自定义属性
        /// <summary>
        /// 订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 代收货款受益人ID
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool? IsReciveCharge { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WayBillID { get; set; }

        /// <summary>
        /// 出库条件
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        #endregion
    }
}
