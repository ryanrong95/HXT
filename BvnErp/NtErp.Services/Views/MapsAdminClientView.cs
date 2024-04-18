using Layer.Data.Sqls;
using NtErp.Services.Models;
using Needs.Linq;
using System.Linq;

namespace NtErp.Services.Views
{
    public class MapsAdminClientView : QueryView<MapsAdminClient, BvnErpReponsitory>
    {
        protected override IQueryable<MapsAdminClient> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdminClient>()
                   select new MapsAdminClient
                   {
                       AdminID = entity.AdminID,
                       ClientID = entity.ClientID
                   };

        }
    }
}
