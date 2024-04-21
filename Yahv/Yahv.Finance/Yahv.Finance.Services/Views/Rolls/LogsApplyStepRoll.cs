using System;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 审批日志视图
    /// </summary>
    public class LogsApplyStepRoll : UniqueView<Logs_ApplyStep, PvFinanceReponsitory>
    {
        public LogsApplyStepRoll()
        {
        }

        public LogsApplyStepRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_ApplyStep> GetIQueryable()
        {
            var logsView = new LogsApplyStepOrigin(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from entity in logsView
                   join admin in adminsView on entity.ApproverID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Logs_ApplyStep()
                   {
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Status = entity.Status,
                       Type = entity.Type,
                       ApplyID = entity.ApplyID,
                       Summary = entity.Summary,
                       ApproverID = entity.ApproverID,
                       Approver = admin
                   };
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public void Enter(ApplyType applyType, ApprovalStatus status, string applyID, string approverID, string summary = "")
        {
            new Logs_ApplyStep()
            {
                ApplyID = applyID,
                ApproverID = approverID,
                Type = applyType,
                Status = status,
                Summary = string.IsNullOrWhiteSpace(summary) ? null : summary,
            }.Enter();
        }
    }
}