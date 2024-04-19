using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Models.Carts
{
    /// <summary>
    /// 来源
    /// </summary>
    public enum CartSource
    {
        /// <summary>
        /// 网站
        /// </summary>
        WebSite = 1,
        /// <summary>
        /// 询价按钮
        /// </summary>
        Inquiry = 2,
        /// <summary>
        /// 询报价
        /// </summary>
        Report = 3,
        /// <summary>
        /// 后台ERP加入购物车
        /// </summary>
        ForCart = 4,
        /// <summary>
        /// 后台ERP加入订单
        /// </summary>
        ForOrder = 5
    }
}
