using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 库存类型
    /// </summary>
    public enum StorageType
    {
        [Description("库存库")]
        Store = 1,

        [Description("流水库")]
        Flow = 2,

        [Description("暂存库")]
        Park = 3,

        [Description("在途库")]
        Ordering = 4,

        [Description("报废库")]
        Scrap = 100
    }
}
