using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 部门类型
    /// </summary>
    public enum DepartmentType
    {
        [Description("审计部")]
        审计部 = 1,
        [Description("业务一部")]
        业务部 = 2,
        [Description("财务部")]
        财务部 = 3,
        [Description("行政部")]
        行政部 = 4,
        [Description("关务部")]
        关务部 = 5,
        [Description("仓库部")]
        仓库部 = 6,
        [Description("风控部")]
        风控部 = 7,
        [Description("信息IT部")]
        信息IT部 = 8,
        [Description("管理部门")]
        管理部门 = 99,

        [Description("业务二部")]
        业务二部 = 10,
        [Description("业务三部")]
        业务三部 = 11,
    }
}
