using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views
{
    public class ClientStorageRoll : StoragesAlls
    {
        string clientID = string.Empty;

        public ClientStorageRoll(string clientID)
        {
            this.clientID = clientID;
        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.ClientID == this.clientID);
            return linq;
        }
    }
}
