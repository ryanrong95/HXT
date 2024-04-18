using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsClient.Model;

namespace Yahv.PvWsClient.Views
{
    public class UserToken : UsersAlls
    {
        string token;

        protected internal UserToken(string token)
        {
            this.token = token;
        }

        protected override IQueryable<ClientUser> GetIQueryable()
        {
            return from user in base.GetIQueryable()
                   join token in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.UserTokens>()
                   on user.ID equals token.UserID
                   where token.Token == this.token
                   select new ClientUser
                   {
                       ID = user.ID,
                       RealName = user.RealName,
                       UserName = user.UserName,
                       Password = user.Password,
                       XDTClientName = user.XDTClientName,
                       XDTClientID = user.XDTClientID,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       Email = user.Email,
                       IsValid = user.IsValid,
                       XDTClientType = user.XDTClientType,
                       UserStatus = user.UserStatus,
                       MobileLastLoginDate = user.MobileLastLoginDate,

                       TokenCreateDate = token.CreateDate,
                   };
        }
    }
}
