using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class WsOrderInput : Yahv.Linq.IUnique
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 代付货款受益人ID
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public bool? IsPayCharge { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WayBillID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 入库条件
        /// </summary>
        public string Conditions { get; set; }
    }
}
