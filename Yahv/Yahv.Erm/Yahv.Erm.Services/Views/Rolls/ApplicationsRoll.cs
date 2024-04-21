using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Roll
{
    /// <summary>
    /// 审批中的申请视图
    /// </summary>
    public class ApplicationsRoll : UniqueView<Application, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationsRoll() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ApplicationsRoll(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Application> GetIQueryable()
        {
            var applyView = new Origins.ApplicationsAll(this.Reponsitory);
            var stepView = new Origins.ApplyVoteStepsAll(this.Reponsitory);
            var currentStepView = stepView.Where(item => item.IsCurrent == true);

            return from entity in applyView
                   join step in stepView on entity.ID equals step.ApplicationID into steps
                   join currentstep in currentStepView on entity.ID equals currentstep.ApplicationID into currentsteps
                   from currentstep in currentsteps.DefaultIfEmpty()
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
                       VoteFlow = entity.VoteFlow,
                       ApplicationType = entity.ApplicationType,
                       Applicant = entity.Applicant,

                       CurrentVoteStep = currentstep,
                       Steps = steps,
                   };
        }
    }
}