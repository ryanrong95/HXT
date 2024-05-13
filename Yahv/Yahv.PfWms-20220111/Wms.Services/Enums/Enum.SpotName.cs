using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services.Enums
{
    public enum SpotName
    {
        /// <summary>
        /// 代报关 下单
        /// </summary>
        [Description("下单")]
        DOrdered = 1,

        /// <summary>
        /// 代报关 归类报价后
        /// </summary>
        [Description("归类")]
        Classified = 2,

        /// <summary>
        /// 代报关 货到香港
        /// </summary>
        [Description("货到香港")]
        ArriveHK = 3,

        /// <summary>
        /// 代报关 报关过关
        /// </summary>
        [Description("报关过关")]
        Declare = 4,

        /// <summary>
        /// 代报关 深圳出库后
        /// </summary>
        [Description("深圳出库后")]
        DSZSend = 5,

        /// <summary>
        /// 代报关 开票后
        /// </summary>
        [Description("开票后")]
        Invoiced = 6,

        /// <summary>
        /// 代仓储 受理后
        /// </summary>
        [Description("受理后")]
        Taked = 7,

        /// <summary>
        /// 代仓储 香港收货后
        /// </summary>
        [Description("收货后")]
        GoodsArrived = 8,

        /// <summary>
        /// 代仓储 香港出库后
        /// </summary>
        [Description("香港出库后")]
        HKSend = 9,
    }
}
