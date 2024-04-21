using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 审批列表视图
    /// </summary>
    public class ApprovalsStatisticsView : QueryView<ApprovalStatistic, PvbErmReponsitory>
    {
        public ApprovalsStatisticsView()
        {

        }

        public ApprovalsStatisticsView(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApprovalStatistic> GetIQueryable()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ApprovalsStatisticsView>()
                   join _applicant in admins on entity.ApplicantID equals _applicant.ID into joinApplicant
                   from applicant in joinApplicant.DefaultIfEmpty()
                   join _approve in admins on entity.ApproveID equals _approve.ID into joinApprove
                   from approve in joinApprove.DefaultIfEmpty()
                   select new ApprovalStatistic()
                   {
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ApplicantID = entity.ApplicantID,
                       Title = entity.Title,
                       Context = entity.Context,
                       Type = (ApplicationType)entity.Type,
                       VoteFlowID = entity.VoteFlowID,
                       Applicant = applicant.RealName,
                       Status = (ApplicationStatus)entity.Status,
                       ApproveID = entity.ApproveID,
                       ApplicationID = entity.ApplicationID,
                       ApplyVoteStepsID = entity.ApplyVoteStepsID,
                       PositionID = entity.PositionID,
                       Uri = entity.Uri,
                       VoteFlowName = entity.VoteFlowName,
                       VoteFlowsID = entity.VoteFlowID,
                       VoteStepID = entity.VoteStepID,
                       VoteStepName = entity.VoteStepName,
                       ApproveName = approve.RealName,
                   };
        }
    }
}