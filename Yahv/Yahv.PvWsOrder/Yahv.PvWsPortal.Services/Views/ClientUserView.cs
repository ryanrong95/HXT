using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsPortal.Services.Models;

namespace Yahv.PvWsPortal.Services.Views
{
    public class ClientUserView : UniqueView<Models.ClientUser, ScCustomReponsitory>
    {
        public ClientUserView()
        {

        }

        protected override IQueryable<ClientUser> GetIQueryable()
        {
            var users = Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>();

            return from user in users
                   select new ClientUser
                   {
                       ID = user.ID,
                       ClientID = user.ClientID,
                       OpenID = user.OpenID,
                       Name = user.Name,
                       RealName = user.RealName,
                       Password = user.Password,
                       Mobile = user.Mobile,
                       Email = user.Email,
                       IsMain = user.IsMain,
                       AdminID = user.AdminID,
                       Status = user.Status,
                       CreateDate = user.CreateDate,
                       UpdateDate = user.UpdateDate,
                       Summary = user.Summary,
                   };
        }

        public ClientUser GetByToken(string token)
        {
            var userTokens = Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.UserTokens>();
            var users = Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>();

            return (from userToken in userTokens
                    join user in users on userToken.UserID equals user.ID
                    where user.Status == 200
                       && userToken.Token == token
                    select new ClientUser
                    {
                        ID = user.ID,
                        RealName = user.RealName,
                        Name = user.Name,
                    }).FirstOrDefault();
        }

    }
}
