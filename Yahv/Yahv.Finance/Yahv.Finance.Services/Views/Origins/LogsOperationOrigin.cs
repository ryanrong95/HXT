using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 操作日志视图
    /// </summary>
    public class LogsOperationOrigin : UniqueView<Logs_Operation, PvFinanceReponsitory>
    {
        internal LogsOperationOrigin()
        {
        }

        internal LogsOperationOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Logs_Operation> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Logs_Operation>()
                   select new Logs_Operation()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Type = entity.Type,
                       Modular = entity.Modular,
                       Operation = entity.Operation,
                       Remark = entity.Remark,
                   };
        }
    }
}