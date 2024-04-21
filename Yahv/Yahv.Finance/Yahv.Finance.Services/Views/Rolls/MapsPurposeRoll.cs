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
    public class MapsPurposeRoll : QueryView<MapsPurpose, PvFinanceReponsitory>
    {
        protected override IQueryable<MapsPurpose> GetIQueryable()
        {
            return new MapsPurposeOrigin();
        }

        public void BatchEnter(string accountID, MapsPurpose[] mapsPurposes)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.MapsPurpose>(item => item.AccountID == accountID);

                if (mapsPurposes.Length <= 0) return;
                reponsitory.Insert<Layers.Data.Sqls.PvFinance.MapsPurpose>(mapsPurposes.Select(t => new Layers.Data.Sqls.PvFinance.MapsPurpose
                {
                    AccountID = t.AccountID,
                    AccountPurposeID = t.AccountPurposeID,
                }));
            }
        }
    }
}
