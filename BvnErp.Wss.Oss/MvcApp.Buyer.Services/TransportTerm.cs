using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services
{
    /// <summary>
    /// 承运方式
    /// </summary>
    public enum TransportTerm
    {
        /// <summary>
        /// 自提
        /// </summary>
        SelfPickUp = 1,
        /// <summary>
        /// UPS
        /// </summary>
        UPS = 2,
        /// <summary>
        /// FedEx
        /// </summary>
        FedEx = 3,
        /// <summary>
        /// DHL
        /// </summary>
        DHL = 4,
        /// <summary>
        /// 顺丰
        /// </summary>
        SF = 5,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 6,
    }


    
}
