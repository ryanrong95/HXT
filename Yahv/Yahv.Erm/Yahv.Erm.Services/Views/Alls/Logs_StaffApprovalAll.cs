using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Alls
{
    public class Logs_StaffApprovalAll : UniqueView<Logs_StaffApproval, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_StaffApprovalAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public Logs_StaffApprovalAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Logs_StaffApproval> GetIQueryable()
        {
            var logsView = new Logs_StaffApprovalOrigin(this.Reponsitory);
            var staffView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>();
            var personalView = new PersonalsOrigin(this.Reponsitory);
            var adminView = new AdminsOrigin(this.Reponsitory);

            return from entity in logsView
                   join staff in staffView on entity.StaffID equals staff.ID
                   join personal in personalView on entity.StaffID equals personal.ID
                   join admin in adminView on entity.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   select new Logs_StaffApproval()
                   {
                       ID = entity.ID,
                       StaffID = entity.StaffID,
                       AdminID = entity.AdminID,
                       ApprovalStep = entity.ApprovalStep,
                       ApprovalStatus = entity.ApprovalStatus,
                       NoticeReportStatus = entity.NoticeReportStatus,
                       EntryReportStatus = entity.EntryReportStatus,
                       Context = entity.Context,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,

                       Admin = admin,
                       Staff = new Staff()
                       {
                           ID = staff.ID,
                           Name = staff.Name,
                           Code = staff.Code,
                           SelCode = staff.SelCode,
                           Gender = (Gender)staff.Gender,
                           DyjCompanyCode = staff.DyjCompanyCode,
                           DyjDepartmentCode = staff.DyjDepartmentCode,
                           DyjCode = staff.DyjCode,
                           WorkCity = staff.WorkCity,
                           LeagueID = staff.LeagueID,
                           PostionID = staff.PostionID,
                           DepartmentCode = staff.DepartmentCode,
                           PostionCode = staff.PostionCode,
                           AssessmentMethod = staff.AssessmentMethod,
                           AssessmentTime = staff.AssessmentTime,
                           AdminID = staff.AdminID,
                           UpdateDate = staff.UpdateDate,
                           CreateDate = staff.CreateDate,
                           Status = (StaffStatus)staff.Status,
                           RegionID = staff.RegionID,
                           SchedulingID = staff.SchedulingID,

                           Personal = personal,
                       }
                   };
        }

        /// <summary>
        /// 获取所有应聘人员所在的步骤和状态
        /// </summary>
        /// <returns></returns>
        public IQueryable<Logs_StaffApproval> GetIQueryableEx()
        {
            var logsView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_StaffApprovals>();
            var linq = from entity in logsView
                       group entity by entity.StaffID into g
                       select new
                       {
                           StaffID = g.Key,
                           ApprovalStep = g.Max(entity => entity.ApprovalStep),
                       };

            var staffView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>();
            var personalView = new PersonalsOrigin(this.Reponsitory);
            var adminView = new AdminsOrigin(this.Reponsitory);

            return from item in linq
                   join entity in logsView on new { item.StaffID, item.ApprovalStep } equals new { entity.StaffID, entity.ApprovalStep }
                   join staff in staffView on entity.StaffID equals staff.ID
                   join personal in personalView on entity.StaffID equals personal.ID
                   join admin in adminView on entity.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
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

                       Admin = admin,
                       Staff = new Staff()
                       {
                           ID = staff.ID,
                           Name = staff.Name,
                           Code = staff.Code,
                           SelCode = staff.SelCode,
                           Gender = (Gender)staff.Gender,
                           DyjCompanyCode = staff.DyjCompanyCode,
                           DyjDepartmentCode = staff.DyjDepartmentCode,
                           DyjCode = staff.DyjCode,
                           WorkCity = staff.WorkCity,
                           LeagueID = staff.LeagueID,
                           PostionID = staff.PostionID,
                           DepartmentCode = staff.DepartmentCode,
                           PostionCode = staff.PostionCode,
                           AssessmentMethod = staff.AssessmentMethod,
                           AssessmentTime = staff.AssessmentTime,
                           AdminID = staff.AdminID,
                           UpdateDate = staff.UpdateDate,
                           CreateDate = staff.CreateDate,
                           Status = (StaffStatus)staff.Status,
                           RegionID = staff.RegionID,
                           SchedulingID = staff.SchedulingID,

                           Personal = personal,
                       }
                   };
        }
    }
}
