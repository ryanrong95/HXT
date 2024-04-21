using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 审批日志 原始视图
    /// </summary>
    public class LogsApplyStepOrigin : UniqueView<Logs_ApplyStep, PvFinanceReponsitory>
    {
        internal LogsApplyStepOrigin()
        {
        }

        internal LogsApplyStepOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_ApplyStep> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Logs_ApplyStep>()
                   select new Logs_ApplyStep()
                   {
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       Status = (ApprovalStatus)entity.Status,
                       Type = (ApplyType)entity.Type,
                       ApplyID = entity.ApplyID,
                       Summary = entity.Summary,
                       ApproverID = entity.ApproverID,
                   };
        }
    }
}