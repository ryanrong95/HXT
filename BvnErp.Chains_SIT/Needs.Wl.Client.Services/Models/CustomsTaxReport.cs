using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Client.Services.Models
{
    /// <summary>
    /// 海关关税、增值税报表
    /// </summary>
    public class CustomsTaxReport : Needs.Wl.Models.DecTaxFlow
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }
    }
}
