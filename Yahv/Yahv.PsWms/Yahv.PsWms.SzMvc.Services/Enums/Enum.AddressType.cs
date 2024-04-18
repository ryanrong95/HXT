using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    public enum AddressType
    {
        /// <summary>
        /// 收货地址
        /// </summary>
        [Description("收货地址")]
        Consignee = 1,

        /// <summary>
        /// 交货地址
        /// </summary>
        [Description("交货地址")]
        Consignor = 2,
    }
}
