using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Models;
using Yahv.PvRoute.Services.Views.Origins;

namespace Yahv.PvRoute.Services.Views.Rolls
{
    public class TransportConsigneesRoll : QueryView<TransportConsignee, PvRouteReponsitory>
    {
        public TransportConsigneesRoll()
        {

        }

        public TransportConsigneesRoll(PvRouteReponsitory reponsitory, IQueryable<TransportConsignee> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<TransportConsignee> GetIQueryable()
        {
            var transportConsigneeOrigin = new TransportConsigneeOrigin(this.Reponsitory);

            return transportConsigneeOrigin;
        }
    }
}
