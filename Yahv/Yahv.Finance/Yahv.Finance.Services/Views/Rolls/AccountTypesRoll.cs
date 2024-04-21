using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class AccountTypesRoll : QueryView<AccountType, PvFinanceReponsitory>
    {
        public AccountTypesRoll() { }

        protected override IQueryable<AccountType> GetIQueryable()
        {
            return new AccountTypesOrigin(this.Reponsitory);
        }
    }
}
