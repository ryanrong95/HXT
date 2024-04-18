using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class CarriersView : UniqueView<Models.Carrier, ScCustomsReponsitory>
    {
        public CarriersView()
        {

        }

        public CarriersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Carrier> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>()
                   select new Models.Carrier
                   {
                       ID = entity.ID,
                       ContactID = entity.ContactID,
                       CarrierType = (Enums.CarrierType)entity.CarrierType,
                       Code = entity.Code,
                       QueryMark = entity.QueryMark,
                       Name = entity.Name,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary,
                       Address = entity.Address,
                   };
        }

        public Models.Carrier FirstOrDefault()
        {
            return this.GetIQueryable().FirstOrDefault();
        }
    }
}
