using System;

namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 审批视图
    /// </summary>
    public class ApprovalStatistic
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 审批流程ID
        /// </summary>
        public string VoteFlowID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public ApplicationType Type { get; set; }

        /// <summary>
        /// 申请流程名称
        /// </summary>
        public string VoteFlowName { get; set; }

        /// <summary>
        /// 申请流程ID
        /// </summary>
        public string VoteFlowsID { get; set; }

        /// <summary>
        /// 申请审批ID
        /// </summary>
        public string ApplyVoteStepsID { get; set; }

        /// <summary>
        /// 审批步骤ID
        /// </summary>
        public string VoteStepID { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproveID { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproveName { get; set; }

        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string VoteStepName { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PositionID { get; set; }

        /// <summary>
        /// uri
        /// </summary>
        public string Uri { get; set; }
    }
}