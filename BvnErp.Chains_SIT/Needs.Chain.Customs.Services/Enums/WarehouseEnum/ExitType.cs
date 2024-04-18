using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 出库类型
    /// </summary>
    public enum ExitType
    {
        [Description("自提")]
        PickUp = 1,
        [Description("送货上门")]
        Delivery = 2,
        [Description("快递")]
        Express = 4
    }

    public enum IsPrint
    {
        [Description("未打印")]
        UnPrint = 0,
        [Description("已打印")]
        Printed = 1,
    }
}
