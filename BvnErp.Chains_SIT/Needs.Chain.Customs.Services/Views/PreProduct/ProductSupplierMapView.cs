using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.PreProduct
{
    public class ProductSupplierMapView : UniqueView<Models.ProductSupplierMap, ScCustomsReponsitory>
    {
        public ProductSupplierMapView()
        {
        }

        internal ProductSupplierMapView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductSupplierMap> GetIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductSupplierMap>()
                   select new Models.ProductSupplierMap
                   {
                       ID = map.ID,
                       SupplierID = map.SupplierID
                   };
        }
    }
}
