using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 订单应收，实际应收
    /// </summary>
    public class VoucherStatistic : Yahv.Services.Models.VoucherStatistic,IUnique
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

