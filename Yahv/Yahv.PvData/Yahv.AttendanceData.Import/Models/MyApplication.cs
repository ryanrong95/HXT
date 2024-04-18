using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Enums;

namespace Yahv.AttendanceData.Import.Models
{
    /// <summary>
    /// 申请
    /// </summary>
    public class MyApplication
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批流
        /// </summary>
        public MyVoteFlow VoteFlow { get; set; }

        /// <summary>
        /// 申请名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 申请内容
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 创建人
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

        #endregion

        #region 扩展属性

        /// <summary>
        /// 个人日程安排类型
        /// </summary>
        public SchedulePrivateType PrivateType { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 部门负责人ID
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        /// 公务、公差、请假的日期
        /// </summary>
        public DateTime AttendDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
