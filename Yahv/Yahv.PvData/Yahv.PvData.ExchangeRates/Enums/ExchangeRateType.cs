using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PvData.ExchangeRates
{
    /// <summary>
    /// 汇率类型
    /// </summary>
    public enum ExchangeRateType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 海关汇率
        /// </summary>
        [Description("海关汇率")]
        Custom = 1,

        /// <summary>
        /// 实时汇率
        /// </summary>
        [Description("实时汇率")]
        RealTime = 2,

        /// <summary>
        /// 约定汇率
        /// </summary>
        [Description("约定汇率")]
        Agreed = 3,

        /// <summary>
        /// 9:30实时汇率
        /// </summary>
        [Description("9:30实时汇率")]
        NineRealTime = 4,
    }
}
