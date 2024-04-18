using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 处理状态
    /// </summary>
    public enum ExcuteStatus
    {
        [Description("香港待提货")]
        香港待提货 = 99,

        [Description("待归类")]
        待归类 = 100,

        [Description("香港待入库")]
        香港待入库 = 101,

        [Description("香港部分到货")]
        香港部分到货 = 102,

        [Description("香港入库完成")]
        香港入库完成 = 103,

        [Description("香港分拣异常")]
        香港分拣异常 = 104,

        [Description("香港待装箱")]
        香港待装箱 = 105,

        [Description("香港待出库")]
        香港待出库 = 106,

        [Description("香港已出库")]
        香港已出库 = 107,

        [Description("正在申报")]
        正在申报 = 108,

        [Description("申请通过")]
        申请通过 = 109,

        [Description("香港已起运")]
        香港已起运 = 110,

        [Description("深圳已入库")]
        深圳已入库 = 120,

        [Description("深圳待出库")]
        深圳待出库 = 121,

        [Description("深圳已出库")]
        深圳已出库 = 122,

        [Description("客户已签收")]
        客户已签收 = 200,
    }
}
