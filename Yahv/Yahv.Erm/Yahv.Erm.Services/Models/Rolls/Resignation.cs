using System;
using Yahv.Linq;

namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 辞职
    /// </summary>
    public class Resignation
    {
        #region 属性

        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostionName { get; set; }

        /// <summary>
        /// 部门负责人
        /// </summary>
        public string DeptLeader { get; set; }

        /// <summary>
        /// 部门负责人ID
        /// </summary>
        public string DeptLeaderID { get; set; }

        /// <summary>
        /// 总经理审批
        /// </summary>
        public string GeneralManager { get; set; }

        /// <summary>
        /// 总经理审批ID
        /// </summary>
        public string GeneralManagerID { get; set; }

        /// <summary>
        /// 承接人
        /// </summary>
        public string Handover { get; set; }

        /// <summary>
        /// 承接人ID
        /// </summary>
        public string HandoverID { get; set; }

        /// <summary>
        /// 离职原因
        /// </summary>
        public string ReasonDescription { get; set; }

        /// <summary>
        /// 工作描述
        /// </summary>
        public string JobDescription { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? ResignationDate { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ID { get; set; }
        #endregion
    }
}