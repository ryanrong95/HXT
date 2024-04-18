using System;
using System.Collections.Generic;
using Needs.Utils.Descriptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
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
}
