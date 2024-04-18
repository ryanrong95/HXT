using Needs.Wl.User.Plat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class UserClientOrignView : Needs.Wl.Models.Views.ClientsView
    {
        IPlatUser User;

        public UserClientOrignView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Models.Client> GetIQueryable()
        {
            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   where client.ID == this.User.ClientID
                   select new Needs.Wl.Models.Client
                   {
                       ID = client.ID,
                       ClientType = (Needs.Wl.Models.Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Needs.Wl.Models.Enums.ClientRank)client.ClientRank,
                       CreateDate = client.CreateDate,
                       Summary = client.Summary,
                       IsValid = client.IsValid
                   };
        }
    }
}
