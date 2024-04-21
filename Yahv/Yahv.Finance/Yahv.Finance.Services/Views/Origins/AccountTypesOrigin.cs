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
    public class AccountTypesOrigin : UniqueView<AccountType, PvFinanceReponsitory>
    {
        internal AccountTypesOrigin() { }

        internal AccountTypesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<AccountType> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountTypes>()
                   select new AccountType()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                   };
        }
    }
}
