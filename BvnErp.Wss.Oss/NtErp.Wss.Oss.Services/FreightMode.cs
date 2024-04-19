using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 运费支付方式
    /// </summary>
    public enum FreightMode
    {
        /// <summary>
        /// 托运人支付
        /// </summary>
        [Description("托运人支付", "装运港托运人支付", "CIF", "CFR")]
        Prepaid = 1,
        /// <summary>
        /// 收货人支付
        /// </summary>
        [Description("收货人支付", "目的港收货人支付", "FOB")]
        Collect = 2
    }


}
