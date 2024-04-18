using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.Enums
{
    public enum WaybillStatusType
    {

        [Description("执行状态")]
        ExecutionStatus = 3,

        [Description("确认收货状态")]
        ConfirmReceiptStatus = 4

    }
}
