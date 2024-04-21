using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Models;

namespace Yahv.PvRoute.Services.Views.Origins
{
    public class TransportConsigneeOrigin : QueryView<TransportConsignee, PvRouteReponsitory>
    {

        internal TransportConsigneeOrigin()
        {

        }

        internal TransportConsigneeOrigin(PvRouteReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<TransportConsignee> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.TransportConsignees>()
                   select new TransportConsignee()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Phone = entity.Phone,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Address = entity.Address,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
