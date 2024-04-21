using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.WorkFlow.Services
{
    /// <summary>
    /// 审批流模型
    /// </summary>
    public class VoteFlow
    {
        /// <summary>
        /// 审批流ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批流类型
        /// </summary>
        public VoteFlowType Type { get; set; }

        /// <summary>
        /// 请假天数上限
        /// </summary>
        public float? UpperLimit { get; set; }

        /// <summary>
        /// 请假天数下限
        /// </summary>
        public float? LowerLimit { get; set; }
    }
}
