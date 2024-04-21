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
    public class MapsAccountTypeOrigin : UniqueView<MapsAccountType, PvFinanceReponsitory>
    {
        internal MapsAccountTypeOrigin() { }

        internal MapsAccountTypeOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<MapsAccountType> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MapsAccountType>()
                   select new MapsAccountType()
                   {
                       AccountID = entity.AccountID,
                       AccountTypeID = entity.AccountTypeID,
                   };
        }
    }
}
