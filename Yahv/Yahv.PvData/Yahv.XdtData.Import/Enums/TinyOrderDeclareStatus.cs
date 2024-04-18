using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.XdtData.Import.Enums
{
    /// <summary>
    /// (新的)装箱单状态
    /// </summary>
    public enum TinyOrderDeclareStatus
    {
        [Description("已装箱")]
        Boxed = 20,

        /// <remarks>
        /// 香港点击申报后
        /// </remarks>
        [Description("申报中")]
        Declaring = 30,

        /// <remarks>
        /// 收到报关的出库通知后
        /// </remarks>
        [Description("待装运")]
        Shiping = 40,

        /// <remarks>
        /// 香港装运后
        /// </remarks>
        [Description("已装运")]
        Shiped = 50,
    }
}
