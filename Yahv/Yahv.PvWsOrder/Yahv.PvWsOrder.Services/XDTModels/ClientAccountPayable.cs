using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    public class ClientAccountPayable : IUnique
    {
        /// <summary>
        /// 主键ID/CompanyID（不是客户ID）
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 货款应付款
        /// </summary>
        public decimal ProductPayable { get; set; }

        /// <summary>
        /// 税款应付款
        /// </summary>
        public decimal TaxPayable { get; set; }

        /// <summary>
        /// 代理费应付款
        /// </summary>
        public decimal AgencyPayable { get; set; }

        /// <summary>
        /// 杂费应付款
        /// </summary>
        public decimal IncidentalPayable { get; set; }
    }
}
