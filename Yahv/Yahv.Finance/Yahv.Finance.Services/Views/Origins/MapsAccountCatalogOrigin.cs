using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;

namespace Yahv.Finance.Services.Views.Origins
{
    public class MapsAccountCatalogOrigin : QueryView<MapsAccountCatalog, PvFinanceReponsitory>
    {
        internal MapsAccountCatalogOrigin()
        {

        }

        internal MapsAccountCatalogOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<MapsAccountCatalog> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.MapsAccountCatalog>()
                   select new MapsAccountCatalog()
                   {
                       AdminID = entity.AdminID,
                       AccountCatalogID = entity.AccountCatalogID
                   };
        }
    }
}