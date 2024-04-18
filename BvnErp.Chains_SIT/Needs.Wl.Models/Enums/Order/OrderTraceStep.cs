using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    /// <summary>
    /// 代理订单轨迹
    /// </summary>
    public enum OrderTraceStep
    {
        [Description("已下单")]
        Submitted = 1,

        [Description("订单处理中")]
        Processing = 2,

        [Description("香港仓库处理中")]
        HKProcessing = 3,

        [Description(" 报关中")]
        Declaring = 4,

        [Description("运输中")]
        InTransit = 5,

        [Description("深圳库房处理中")]
        SZProcessing = 6,

        [Description("派送中")]
        Delivering = 7,

        [Description("已提货")]
        PickUp = 8,

        [Description("订单已完成")]
        Completed = 9,

        [Description("订单异常")]
        Anomaly = 10,
    }
}
