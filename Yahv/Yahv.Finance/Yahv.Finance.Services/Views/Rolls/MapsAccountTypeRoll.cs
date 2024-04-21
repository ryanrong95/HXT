using Layers.Data.Sqls;
using Layers.Linq;
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
    public class MapsAccountTypeRoll : QueryView<MapsAccountType, PvFinanceReponsitory>
    {
        public MapsAccountTypeRoll()
        {

        }

        public MapsAccountTypeRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MapsAccountType> GetIQueryable()
        {
            var maps = new MapsAccountTypeOrigin(this.Reponsitory);
            var accountTypes = new AccountTypesOrigin(this.Reponsitory);

            return from entity in maps
                   join accountType in accountTypes on entity.AccountTypeID equals accountType.ID into _accountType
                   from accountType in _accountType.DefaultIfEmpty()
                   select new MapsAccountType()
                   {
                       ID = entity.ID,
                       AccountID = entity.AccountID,
                       AccountTypeID = entity.AccountTypeID,
                       AccountTypeName = accountType.Name,
                   };
        }

        public void BatchEnter(string accountID, MapsAccountType[] mapsAccountTypes)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.MapsAccountType>(item => item.AccountID == accountID);

                if (mapsAccountTypes.Length <= 0) return;
                reponsitory.Insert<Layers.Data.Sqls.PvFinance.MapsAccountType>(mapsAccountTypes.Select(t => new Layers.Data.Sqls.PvFinance.MapsAccountType
                {
                    AccountID = t.AccountID,
                    AccountTypeID = t.AccountTypeID,
                }));
            }
        }
    }
}
