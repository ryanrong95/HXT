using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.WorkFlow.Services.Models.Origins
{
    /// <summary>
    /// 申请
    /// </summary>
    public class Application : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批流
        /// </summary>
        public string VoteFlowID { get; set; }

        /// <summary>
        /// 申请名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 申请内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 实际申请人
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 创建人: 申请人或代理人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 申请状态： 废弃、草稿、驳回、审批中、已完成 
        /// </summary>
        public ApplicationStatus Status { get; set; }

        #endregion
    }
}
