using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 部门类型
    /// </summary>
    public enum DepartmentType
    {
        [Description("审计部")]
        审计部 = 1,
        [Description("业务部")]
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
    }

    /// <summary>
    /// 职务类型
    /// </summary>
    public enum PostType
    {
        [Description("普通员工")]
        Staff = 100,
        [Description("部门负责人")]
        Manager = 200,
        [Description("总经理")]
        President = 300,
    }
}
