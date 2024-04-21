using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.WorkFlow.Services
{
    /// <summary>
    /// 审批流类型
    /// </summary>
    public enum VoteFlowType
    {
        [Description("离职")]
        Dimission = 1,

        [Description("请假")]
        Leave = 2,

        [Description("加班")]
        Overtime = 3,

        /// <summary>
        /// 补签考勤记录: Supplementary Attendance Record
        /// </summary>
        [Description("补签")]
        SAR = 4,
    }
}
