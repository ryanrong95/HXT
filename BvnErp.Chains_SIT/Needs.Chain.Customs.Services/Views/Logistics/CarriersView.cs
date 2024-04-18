using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CarriersView : UniqueView<Models.Carrier, ScCustomsReponsitory>
    {
        public CarriersView()
        {

        }

        internal CarriersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Carrier> GetIQueryable()
        {
            var contactView = new ContactsView(this.Reponsitory);
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>()
                   join contact in contactView on entity.ContactID equals contact.ID into g
                   from contact in g.DefaultIfEmpty()
                 //  where entity.Status == (int)Enums.Status.Normal
                   select new Models.Carrier
                   {
                       ID = entity.ID,
                       Contact = contact,
                       CarrierType=(Enums.CarrierType)entity.CarrierType,
                       Name = entity.Name,
                       Code = entity.Code,
                       QueryMark=entity.QueryMark,
                       Status = (Enums.Status)entity.Status,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Address=entity.Address
                   };
        }
    }
}
