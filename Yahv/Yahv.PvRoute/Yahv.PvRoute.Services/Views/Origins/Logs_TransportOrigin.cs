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
    public class Logs_TransportOrigin : UniqueView<Logs_Transport, PvRouteReponsitory>
    {
        internal Logs_TransportOrigin()
        {

        }

        internal Logs_TransportOrigin(PvRouteReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Logs_Transport> GetIQueryable()
        {
             return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.Logs_Transports>()
                     select new Logs_Transport()
                     {
                         ID = entity.ID,
                         FaceOrderID = entity.FaceOrderID,
                         Summary = entity.Summary,
                         CreateDate = entity.CreateDate,
                         Json = entity.Json,
                         Phone = entity.Phone,
                         Contact = entity.Contact,
                         ConsigneeID = entity.ConsigneeID
                     };
        }
    }
}
