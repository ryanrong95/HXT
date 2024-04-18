using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.Enums
{
    public enum ExpressCarrierType
    {
        /// <summary>
        /// 国际快递
        /// </summary>
        [Description("国际快递")]
        InteExpress = 100,

        /// <summary>
        /// 国际物流
        /// </summary>
        [Description("国际物流")]
        InteLogistics = 200,

        /// <summary>
        /// 国内快递
        /// </summary>
        [Description("国内快递")]
        DomesticExpress = 300,

        /// <summary>
        /// 国内物流
        /// </summary>
        [Description("国内物流")]
        DomesticLogistics = 400,
    }
}
