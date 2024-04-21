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
    public class AccountPurposesOrigin : UniqueView<AccountPurpose, PvFinanceReponsitory>
    {
        internal AccountPurposesOrigin() { }

        internal AccountPurposesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<AccountPurpose> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountPurposes>()
                   select new AccountPurpose()
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
