using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 国际币种规范
    /// </summary>
    public enum Currency
    {
        [Naming("Unkown")]
        Unkown = 0,
        /// <summary>
        /// 人民币
        /// </summary>
        [Naming("CNY¥")]
        CNY = 1,
        /// <summary>
        /// 美元
        /// </summary>
        [Naming("USA$")]
        USD = 2,
        /// <summary>
        /// 港元
        /// </summary>
        [Naming("HK$")]
        HKD = 3,
        /// <summary>
        /// 欧元
        /// </summary>
        [Naming("€")]
        EUR = 4,
        /// <summary>
        /// 英镑
        /// </summary>
        [Naming("￡")]
        GBP = 5,
        /// <summary>
        /// 日元
        /// </summary>
        [Naming("JPY¥")]
        JPY = 6,
        /// <summary>
        /// 澳元
        /// </summary>
        [Naming("A$")]
        AUD = 7,
        /// <summary>
        /// 加元
        /// </summary>
        [Naming("C$")]
        CAD = 8,
        /// <summary>
        /// 新加坡元
        /// </summary>
        [Naming("S$")]
        SGD = 9,
        /// <summary>
        /// 卢比
        /// </summary>
        [Naming("₹")]
        INR = 10,
    }
}
