using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 审批流
    /// </summary>
    public class MyVoteFlow
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 审批类型
        /// </summary>
        public ApplicationType Type { get; set; }

        internal MyVoteFlow() { }

        /// <summary>
        /// 请假天数下限
        /// </summary>
        public decimal? LowerLimit { get; set; }

        /// <summary>
        /// 请假天数上限
        /// </summary>
        public decimal? UpperLimit { get; set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public Step[] Steps { get; internal set; }

        /// <summary>
        /// 审批步骤
        /// </summary>
        public class Step
        {
            public string ID { get; internal set; }
            public string Name { get; internal set; }
            public string AdminID { get; internal set; }
            public int OrderIndex { get; internal set; }
        }
    }
}
