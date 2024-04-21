using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class AccountsOrigin : UniqueView<Account, PvFinanceReponsitory>
    {
        internal AccountsOrigin() { }

        internal AccountsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Account> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Accounts>()
                   select new Account()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       NatureType = (Underly.NatureType)entity.NatureType,
                       ManageType = (Underly.ManageType)entity.ManageType,
                       Currency = (Underly.Currency)entity.Currency,
                       BankName = entity.BankName,
                       OpeningBank = entity.OpeningBank,
                       BankAddress = entity.BankAddress,
                       District = entity.District,
                       SwiftCode = entity.SwiftCode,
                       OpeningTime = entity.OpeningTime,
                       IsHaveU = (entity.IsHaveU) ?? false,
                       BankNo = entity.BankNo,
                       OwnerID = entity.OwnerID,
                       GoldStoreID = entity.GoldStoreID,
                       EnterpriseID = entity.EnterpriseID,
                       PersonID = entity.PersonID,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                       ShortName = entity.ShortName,
                       Summary = entity.Summary,
                       Source = (AccountSource)entity.Source,
                       IsVirtual = entity.IsVirtual,
                       DyjShortName = entity.DyjShortName,
                   };
        }


    }
}
