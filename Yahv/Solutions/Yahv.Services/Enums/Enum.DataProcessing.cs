using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Enums
{
    public enum ProcessingType
    {
        [Description("入库操作")]
        Sorting = 1,
        [Description("出库操作")]
        Picking = 2,
        [Description("租赁操作")]
        LsNotice = 3

    }
}
