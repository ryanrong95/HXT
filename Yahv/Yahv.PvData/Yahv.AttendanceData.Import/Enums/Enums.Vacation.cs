using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.AttendanceData.Import
{
    /// <summary>
    /// 员工假期类型
    /// </summary>
    public enum VacationType
    {
        [Description("年假")]
        YearsDay = 1,

        [Description("调休假")]
        OffDay = 2,

        [Description("病假")]
        SickDay = 3,

        [Description("产检假")]
        ProductionInspectionDay = 4,
    }
}
