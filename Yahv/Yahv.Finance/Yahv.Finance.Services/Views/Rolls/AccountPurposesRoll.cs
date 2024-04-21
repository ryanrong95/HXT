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
    public class AccountPurposesRoll : QueryView<AccountPurpose, PvFinanceReponsitory>
    {
        public AccountPurposesRoll() { }

        protected override IQueryable<AccountPurpose> GetIQueryable()
        {
            return new AccountPurposesOrigin(this.Reponsitory);
        }
    }
}
