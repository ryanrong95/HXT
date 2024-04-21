using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class ClientsRoll : Origins.ClientsOrigin
    {
        public ClientsRoll()
        {
        }
        protected override IQueryable<Client> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
    public class TradingClientsRoll : Origins.TradingClientsOrigin
    {
        public TradingClientsRoll()
        {
        }
        protected override IQueryable<TradingClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
