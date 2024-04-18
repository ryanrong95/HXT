using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 报关使用的中转库房
    /// </summary>
    public enum WarehouseType
    {
        /// <summary>
        /// 香港库房
        /// </summary>
        [Description("香港库房")]
        HongKong = 100,

        /// <summary>
        /// 深圳库房
        /// </summary>
        [Description("深圳库房")]
        ShenZhen = 200,
    }
}
