using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 通过token,查询User
    /// </summary>
    public class UserTokenLogin : View<Models.PlatUser, ScCustomsReponsitory>
    {
        private string Token;

        private string UserName;

        private string IP;

        /// <summary>
        /// Token
        /// </summary>
        /// <param name="token"></param>
        public UserTokenLogin(string token, string ip)
        {
            this.Token = token;
            this.IP = ip;
        }

        /// <summary>
        /// 使用用户名+记住密码生成的Token验证
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="token"></param>
        public UserTokenLogin(string userName, string token, string ip)
        {
            this.UserName = userName;
            this.Token = token;
            this.IP = ip;
        }

        protected override IQueryable<Models.PlatUser> GetIQueryable()
        {
            //var query = from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UserTokens>()
            //            join user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on token.UserID equals user.ID
            //            where token.Token == this.Token && token.IP == this.IP && user.Status == (int)Needs.Wl.Models.Enums.Status.Normal
            //            select new Models.PlatUser
            //            {
            //                ID = user.ID,
            //                ClientID = user.ClientID,
            //                OpenID = user.OpenID,
            //                Mobile = user.Mobile,
            //                Email = user.Email,
            //                UserName = user.Name,
            //                RealName = user.RealName,
            //                Password = user.Password,
            //                IsMain = user.IsMain
            //            };

            //if (string.IsNullOrEmpty(this.UserName))
            //{
            //    query.Where(s => s.UserName == this.UserName);
            //}

            var query = from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                        join token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UserTokens>() on user.ID equals token.UserID
                        where token.Token == this.Token && token.IP == this.IP && user.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                        select user;

            if (string.IsNullOrEmpty(this.UserName))
            {
                query.Where(s => s.Name == this.UserName);
            }

            return query.Select(user => new Models.PlatUser
            {
                ID = user.ID,
                ClientID = user.ClientID,
                OpenID = user.OpenID,
                Mobile = user.Mobile,
                Email = user.Email,
                UserName = user.Name,
                RealName = user.RealName,
                Password = user.Password,
                IsMain = user.IsMain
            });
        }
    }
}