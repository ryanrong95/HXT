using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.Enums;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class BookAccountsOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.BookAccount, Layers.Data.Sqls.PvdCrmReponsitory>
    {
        internal BookAccountsOrigin()
        {


        }
        BookAccountsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<BookAccount> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
          //  var contanctView = new ContactsOrigin(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.BookAccounts>()
                   join enterprise in enterprisesView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Yahv.CrmPlus.Service.Models.Origins.BookAccount
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       RelationType=(RelationType)entity.RelationType,
                       BookAccountType = (BookAccountType)entity.Type,
                       BookAccountMethord = (BookAccountMethord)entity.Methord,
                       Bank =entity.Bank,
                       BankAddress=entity.BankAddress,
                       BankCode=entity.BankCode,
                       Account=entity.Account,
                        Currency=(Currency)entity.Currency,
                       SwiftCode=entity.SwiftCode,
                       Transfer=entity.Transfer,
                       CreatorID=entity.CreatorID,
                       Status = (DataStatus)entity.Status,
                       Admin = admin,
                       IsPersonal=entity.IsPersonal,
                       Enterprise=enterprise
                   };
        }

    }
}
