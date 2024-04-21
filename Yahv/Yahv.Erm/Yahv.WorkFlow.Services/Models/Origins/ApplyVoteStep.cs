using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.WorkFlow.Services.Models.Origins
{
    /// <summary>
    /// 申请审批步骤
    /// </summary>
    public class ApplyVoteStep : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 审批流步骤
        /// </summary>
        public string VoteStepID { get; set; }

        /// <summary>
        /// 是否是当前审批步骤
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 审批状态: 赞同、否决
        /// </summary>
        public ApplyVoteStepStatus Status { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        #region 事件



        #endregion
    }
}
