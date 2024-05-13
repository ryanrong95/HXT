using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services
{
    public enum PrintingType
    {
        [Yahv.Underly.Attributes.Description("标签")]
        Label=100,
        [Yahv.Underly.Attributes.Description("产品")]
        Product = 200
    }

    public enum PrintingStatus
    {
        [Yahv.Underly.Attributes.Description("正常")]
        Normal=200,
        [Yahv.Underly.Attributes.Description("删除")]
        Deleted=400,
    }
}
