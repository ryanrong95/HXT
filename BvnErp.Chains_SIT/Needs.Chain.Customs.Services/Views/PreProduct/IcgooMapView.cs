using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class IcgooMapView : UniqueView<Models.IcgooMap, ScCustomsReponsitory>
    {
        public IcgooMapView()
        {
        }

        internal IcgooMapView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.IcgooMap> GetIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>()
                   select new Models.IcgooMap
                   {
                       ID = map.ID,
                       IcgooOrder = map.IcgooOrder,
                       OrderID = map.OrderID,
                       CreateDate = map.CreateDate,
                       UpdateDate = map.UpdateDate,
                       Status = (Enums.Status)map.Status,
                       CompanyType = (Enums.CompanyTypeEnums)map.CompanyType,
                       Summary = map.Summary,
                   };
        }
    }
}
