using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class BanksOrigin : UniqueView<Bank, PvFinanceReponsitory>
    {
        internal BanksOrigin() { }

        internal BanksOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Bank> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Banks>()
                   select new Bank()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       EnglishName = entity.EnglishName,
                       CostSummay = entity.CostSummay,
                       IsAccountCost = entity.IsAccountCost,
                       AccountCost = entity.AccountCost,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                       SwiftCode = entity.SwiftCode,
                   };
        }
    }
}
