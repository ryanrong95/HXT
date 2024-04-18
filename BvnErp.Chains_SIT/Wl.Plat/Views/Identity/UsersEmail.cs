using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 通过Email 查询User
    /// </summary>
    public sealed class UsersEmail : View<Models.PlatUser, ScCustomsReponsitory>
    {
        private string Email;
        public UsersEmail(string email)
        {
            this.Email = email;
        }

        protected override IQueryable<Models.PlatUser> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                   where user.Email == this.Email && user.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.PlatUser
                   {
                       ID = user.ID,
                       ClientID = user.ClientID,
                       Email = user.Email,
                       UserName = user.Name,
                       RealName = user.RealName,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       OpenID = user.OpenID
                   };
        }
    }
}