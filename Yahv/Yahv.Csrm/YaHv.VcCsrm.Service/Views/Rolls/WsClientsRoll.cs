using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class WsClientsRoll : Views.Origins.WsClientsOrigin
    {
        public WsClientsRoll()
        {
        }
        protected override IQueryable<Models.WsClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
