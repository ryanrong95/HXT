using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 申请视图
    /// </summary>
    public class ApplicationsAll : UniqueView<Application, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationsAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ApplicationsAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Application> GetIQueryable()
        {
            var applicationsView = new ApplicationsOrigin(this.Reponsitory).Where(item => item.ApplicationStatus != ApplicationStatus.Delete);
            var voteflowView = new VoteFlowsOrigin(this.Reponsitory);
            var admins = new AdminsOrigin(Reponsitory);

            return from entity in applicationsView
                   join admin in admins on entity.ApplicantID equals admin.ID
                   join voteflow in voteflowView on entity.VoteFlowID equals voteflow.ID
                   select new Application()
                   {
                       ID = entity.ID,
                       VoteFlowID = entity.VoteFlowID,
                       Title = entity.Title,
                       Context = entity.Context,
                       ApplicantID = entity.ApplicantID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ApplicationStatus = entity.ApplicationStatus,

                       VoteFlow = voteflow,
                       ApplicationType = voteflow.Type,
                       Applicant = admin,
                   };
        }
    }

    /// <summary>
    /// 申请的审批步骤
    /// </summary>
    public class ApplyVoteStepsAll : UniqueView<ApplyVoteStep, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplyVoteStepsAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ApplyVoteStepsAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ApplyVoteStep> GetIQueryable()
        {
            var admins = new AdminsOrigin(Reponsitory);
            var steps = new VoteStepsOrigin(Reponsitory);

            return from entity in this.Reponsitory.ReadTable<ApplyVoteSteps>()
                   join admin in admins on entity.AdminID equals admin.ID
                   join step in steps on entity.VoteStepID equals step.ID
                   select new ApplyVoteStep()
                   {
                       ID = entity.ID,
                       ApplicationID = entity.ApplicationID,
                       VoteStepID = entity.VoteStepID,
                       IsCurrent = entity.IsCurrent,
                       AdminID = entity.AdminID,
                       Status = (ApprovalStatus)entity.Status,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,

                       Admin = admin,
                       VoteStep = step,
                   };
        }
    }

    /// <summary>
    /// 申请的审批日志视图
    /// </summary>
    public class Logs_ApplyVoteStepsAll : UniqueView<Logs_ApplyVoteStep, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_ApplyVoteStepsAll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public Logs_ApplyVoteStepsAll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Logs_ApplyVoteStep> GetIQueryable()
        {
            var logsView = new Logs_ApplyVoteStepsOrigin(this.Reponsitory);
            var stepsView = new VoteStepsOrigin(this.Reponsitory);
            var admins = new AdminsOrigin(Reponsitory);

            var linq = from entity in logsView
                       join admin in admins on entity.AdminID equals admin.ID
                       join steps in stepsView on entity.VoteStepID equals steps.ID
                       select new Logs_ApplyVoteStep()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           VoteStepID = entity.VoteStepID,
                           AdminID = entity.AdminID,
                           Status = entity.Status,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,

                           Admin = admin,
                           VoteStepName = steps.Name,
                       };
            return linq;
        }
    }
}