using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Enums
{
    /// <summary>
    /// 海关税类型
    /// </summary>
    public enum CustomsRateType
    {
        [Description("进口关税")]
        ImportTax = 0,

        [Description("出口关税")]
        ExportTax = 1,

        [Description("增值税")]
        AddedValueTax = 2,

        [Description("消费税")]
        ConsumeTax = 3
    }

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
        /// 九点半汇率
        /// </summary>
        [Description("九点半汇率")]
        NineRealTime = 4,
    }
}
