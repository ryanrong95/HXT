using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 员工的入职审批日志
    /// </summary>
    internal class Logs_StaffApprovalOrigin : UniqueView<Logs_StaffApproval, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal Logs_StaffApprovalOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal Logs_StaffApprovalOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<Logs_StaffApproval> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_StaffApprovals>()
                   select new Logs_StaffApproval()
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       AdminID = entity.AdminID,
                       ApprovalStep = (StaffApprovalStep)entity.ApprovalStep,
                       ApprovalStatus = (StaffApprovalStatus)entity.ApprovalStatus,
                       NoticeReportStatus = (StaffNoticeReportStatus)entity.NoticeReportStatus,
                       EntryReportStatus = (StaffEntryReportStatus)entity.EnterReportStatus,
                       Context = entity.Context,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
