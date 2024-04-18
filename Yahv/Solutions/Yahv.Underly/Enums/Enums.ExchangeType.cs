using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 汇率类型
    /// </summary>
    public enum ExchangeType
    {
        [Description("海关汇率")]
        Customs = 10,
        [Description("实时汇率", "浮动汇率")]
        Floating = 20,
        [Description("固定汇率")]
        Fixed = 30,
        [Description("预设汇率")]
        Preset = 40,
        [Description("上午10点中国银行")]
        TenAmChineseBank = 50,
        [Description("上午9点半中国银行")]
        NineAmChineseBank = 60,
    }
}
