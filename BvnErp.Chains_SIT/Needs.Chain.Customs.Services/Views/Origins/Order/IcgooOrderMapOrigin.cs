using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class IcgooOrderMapOrigin : UniqueView<Models.IcgooOrderMap, ScCustomsReponsitory>
    {
        protected override IQueryable<Models.IcgooOrderMap> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>()
                   select new Models.IcgooOrderMap
                   {
                       ID = entity.ID,
                       IcgooOrder = entity.IcgooOrder,
                       OrderID = entity.OrderID,
                       Status = (Enums.Status)entity.Status,
                       CompanyType = (Enums.CompanyTypeEnums)entity.CompanyType,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
