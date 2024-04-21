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
    public class ApplicationsOrigin : UniqueView<Application, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public ApplicationsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Application> GetIQueryable() 
        {
            return from entity in this.Reponsitory.ReadTable<Applications>()
                   select new Application()
                   {
                       ID = entity.ID,
                       VoteFlowID= entity.VoteFlowID,
                       Title = entity.Title,
                       Context = entity.Context,
                       ApplicantID = entity.ApplicantID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       ApplicationStatus = (ApplicationStatus)entity.Status,
                   };
        }
    }

    /// <summary>
    /// 申请审批日志视图
    /// </summary>
    public class Logs_ApplyVoteStepsOrigin : UniqueView<Logs_ApplyVoteStep, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_ApplyVoteStepsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        public Logs_ApplyVoteStepsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Logs_ApplyVoteStep> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Logs_ApplyVoteSteps>()
                   select new Logs_ApplyVoteStep()
                   {
                       ID = entity.ID,
                       ApplicationID = entity.ApplicationID,
                       VoteStepID = entity.VoteStepID,
                       AdminID = entity.AdminID,
                       Status  = (ApprovalStatus)entity.Status,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}