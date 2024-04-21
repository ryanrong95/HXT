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
    public class MapsPurposeOrigin : UniqueView<MapsPurpose, PvFinanceReponsitory>
    {
        internal MapsPurposeOrigin() { }

        internal MapsPurposeOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<MapsPurpose> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MapsPurpose>()
                   select new MapsPurpose()
                   {
                       AccountID = entity.AccountID,
                       AccountPurposeID = entity.AccountPurposeID,
                   };
        }
    }
}
