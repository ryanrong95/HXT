using NtErp.Wss.Sales.Services.Underly.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly.Premiums
{
    public class OrderPremium : PremiumBase
    {
        /// <summary>
        /// 交货地
        /// </summary>
        public District District { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        /// <example>
        /// 特有的管理
        /// </example>
        public Source Source { get; set; }



        public OrderPremium()
        {

        }
    }
}
