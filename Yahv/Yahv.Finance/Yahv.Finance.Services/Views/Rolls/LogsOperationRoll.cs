using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class LogsOperationRoll : UniqueView<Logs_Operation, PvFinanceReponsitory>
    {
        public LogsOperationRoll()
        {
        }

        public LogsOperationRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_Operation> GetIQueryable()
        {
            var logs = new LogsOperationOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);

            return from log in logs
                   join admin in admins on log.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Logs_Operation()
                   {
                       ID = log.ID,
                       CreateDate = log.CreateDate,
                       CreatorID = log.CreatorID,
                       Type = log.Type,
                       CreatorName = admin.RealName,
                       Modular = log.Modular,
                       Remark = log.Remark,
                       Operation = log.Operation,
                   };
        }
    }
}