using System.Linq;
using System.Runtime.InteropServices;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 工资申请
    /// </summary>
    public class SalaryAppliesRoll : UniqueView<SalaryApply, PvFinanceReponsitory>
    {
        public SalaryAppliesRoll()
        {
        }

        public SalaryAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SalaryApply> GetIQueryable()
        {
            var applies = new SalaryAppliesOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);

            return from apply in applies
                   join admin in admins on apply.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new SalaryApply()
                   {
                       Currency = apply.Currency,
                       ID = apply.ID,
                       CreateDate = apply.CreateDate,
                       CreatorID = apply.CreatorID,
                       Status = apply.Status,
                       Price = apply.Price,
                       Summary = apply.Summary,
                       CallBackID = apply.CallBackID,
                       SenderID = apply.SenderID,
                       CallBackUrl = apply.CallBackUrl,
                       Department = apply.Department,
                       CreatorName = admin.RealName,
                       Title = apply.Title,
                   };
        }
    }
}