using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单本次申请金额
    /// </summary>
    public class OrderCurrentPayAmount : Needs.Wl.Models.Order
    {
        /// <summary>
        /// 该订单中是否有供应商匹配型号
        /// </summary>
        public bool IsMatchSupplier { get; set; }

        /// <summary>
        /// 本次申请金额
        /// </summary>
        public decimal CurrentPaidAmount { get; set; }
    }
}
