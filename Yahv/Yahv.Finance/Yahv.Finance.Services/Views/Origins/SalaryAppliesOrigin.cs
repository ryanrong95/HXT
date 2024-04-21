using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 工资申请 原始视图
    /// </summary>
    public class SalaryAppliesOrigin : UniqueView<SalaryApply, PvFinanceReponsitory>
    {
        internal SalaryAppliesOrigin()
        {
        }

        internal SalaryAppliesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SalaryApply> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SalaryApplies>()
                   select new SalaryApply()
                   {
                       Currency = (Currency)entity.Currency,
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Status = (ApplyStauts)entity.Status,
                       Price = entity.Price,
                       Summary = entity.Summary,
                       SenderID = entity.SenderID,
                       CallBackID = entity.CallBackID,
                       CallBackUrl = entity.CallBackUrl,
                       Title = entity.Title,
                   };
        }
    }
}