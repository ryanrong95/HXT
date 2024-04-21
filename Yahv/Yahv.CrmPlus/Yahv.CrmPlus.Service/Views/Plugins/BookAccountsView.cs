using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Plugins
{
    public class BookAccountsView : UniqueView<BookAccount, PvdCrmReponsitory>
    {
        public BookAccountsView()
        {
        }

        internal BookAccountsView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal BookAccountsView(PvdCrmReponsitory reponsitory, IQueryable<BookAccount> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<BookAccount> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.BookAccounts>()
                   where entity.Status == (int)DataStatus.Normal
                   select new BookAccount
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       RelationType = (RelationType)entity.RelationType,
                       BookAccountType = (BookAccountType)entity.Type,
                       BookAccountMethord = (BookAccountMethord)entity.Methord,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       BankCode = entity.BankCode,
                       Account = entity.Account,
                       Currency = (Currency)entity.Currency,
                       SwiftCode = entity.SwiftCode,
                       Transfer = entity.Transfer,
                       CreatorID = entity.CreatorID,
                       Status = (DataStatus)entity.Status,
                       IsPersonal = entity.IsPersonal,
                   };
        }
    }
}
