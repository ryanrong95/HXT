using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientBalanceViewOrigin : UniqueView<Models.ClientBalance, ScCustomsReponsitory>
    {
        public ClientBalanceViewOrigin()
        {
        }

        internal ClientBalanceViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientBalance> GetIQueryable()
        {
            return from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientBalance>()

                   select new Models.ClientBalance
                   {
                       ID = apply.ID,
                       ClientID = apply.ClientID,
                       ClientAccount = apply.ClientAccount,
                       Balance = apply.Balance,
                       Currency = apply.Currency,
                       Version = apply.Version,
                       Status = (Enums.Status)apply.Status,
                       CreateDate = apply.CreateDate,
                       UpdateDate = apply.UpdateDate,
                       Summary = apply.Summary,
                   };
        }
    }
}
