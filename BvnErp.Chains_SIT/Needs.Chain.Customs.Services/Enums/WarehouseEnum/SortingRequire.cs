using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 分拣要求
    /// </summary>
    public enum SortingRequire
    {
        /// <summary>
        /// 不拆箱
        /// </summary>
        [Description("不拆箱")]
        Packed = 1,

        /// <summary>
        /// 拆箱
        /// </summary>
        [Description("拆箱")]
        UnPacking = 2,
    }
}
