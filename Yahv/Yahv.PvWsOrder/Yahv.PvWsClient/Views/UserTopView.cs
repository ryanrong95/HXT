using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsClient.Model;

namespace Yahv.PvWsClient.Views
{
    public class UserTopView : UniqueView<ClientUser, ScCustomReponsitory>
    {

        protected internal UserTopView()
        {

        }

        protected override IQueryable<ClientUser> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>()
                   join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>() on user.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   join token in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.UserTokens>()
                   on user.ID equals token.UserID
                   orderby token.CreateDate descending
                   select new ClientUser
                   {
                       ID = user.ID,
                       RealName = user.RealName,
                       UserName = user.Name,
                       XDTClientName = company.Name,
                       XDTClientID = user.ClientID,
                       Password = user.Password,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       Email = user.Email,
                       Token = token.Token,
                       IP = token.IP,
                   };
        }
    }
}
