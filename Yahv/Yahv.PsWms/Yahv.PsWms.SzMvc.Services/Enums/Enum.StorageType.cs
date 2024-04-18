using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 库存类型
    /// </summary>
    public enum StorageType
    {
        /// <summary>
        /// 库存库
        /// </summary>
        [Description("库存库")]
        Store = 1,

        /// <summary>
        /// 流水库
        /// </summary>
        [Description("流水库")]
        Flow = 2,

        /// <summary>
        /// 暂存库
        /// </summary>
        [Description("暂存库")]
        Park = 3,

        /// <summary>
        /// 在途库
        /// </summary>
        [Description("在途库")]
        Ordering = 4,

        /// <summary>
        /// 报废库
        /// </summary>
        [Description("报废库")]
        Scrap = 100,
    }
}
