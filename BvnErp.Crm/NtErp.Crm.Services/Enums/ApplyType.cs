using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    public enum ApplyType
    {
        [Description("项目申请")]
        Project = 10,

        [Description("行动申请")]
        Action = 20,

        [Description("客户新增申请")]
        CreatedClient = 30,

        [Description("销售状态改为DI申请")]
        DIApply = 50,

        [Description("销售状态改为DW申请")]
        DWApply = 80,

        [Description("销售状态改为MP申请")]
        MPApply = 100,
    }
}
