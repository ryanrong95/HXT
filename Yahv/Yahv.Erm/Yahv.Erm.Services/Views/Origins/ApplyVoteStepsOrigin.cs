using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 申请审批步骤
    /// </summary>
    public class ApplyVoteStepsOrigin : UniqueView<ApplyVoteStep, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplyVoteStepsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ApplyVoteStepsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<ApplyVoteStep> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<ApplyVoteSteps>()
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
                   };
        }
    }
}