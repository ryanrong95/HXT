using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class UserAccountsView : QueryView<UserAccount, Layer.Data.Sqls.BvOrdersReponsitory>
    {

        public UserAccountsView()
        {
        }



        protected override IQueryable<UserAccount> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOrders.UserAccounts>()
                   select new UserAccount
                   {
                       ID = entity.ID,
                       UserID = entity.UserID,
                       Amount = entity.Amount,
                       Currency = (Currency)entity.Currency,
                       Type = (UserAccountType)entity.Type,
                       Source = (InputSource) entity.Source,
                       Code = entity.Code,
                       CreateDate = entity.CreateDate
                   };

            return linqs;
        }
    }
}
