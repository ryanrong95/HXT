using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 通过token,查询User
    /// </summary>
    public class UserEmailTokenLogin : View<Models.PlatUser, ScCustomsReponsitory>
    {
        private string Token;
        public UserEmailTokenLogin(string token)
        {
            this.Token = token;
        }

        protected override IQueryable<Models.PlatUser> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>()
                   join user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on token.Email equals user.Email
                   where token.Token == this.Token && token.CreateDate >= DateTime.Now.AddHours(-24)
                   select new Models.PlatUser
                   {
                       ID = user.ID,
                       ClientID = user.ClientID,
                       Mobile = user.Mobile,
                       Email = user.Email,
                       UserName = user.Name,
                       RealName = user.RealName,
                       IsMain = user.IsMain,
                       OpenID = user.OpenID
                   };
        }
    }
}