using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 用户登录查询视图
    /// </summary>
    public class UsersLogin : View<Models.PlatUser, ScCustomsReponsitory>
    {
        private string UserName;
        private string Password;

        public UsersLogin(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        protected override IQueryable<Models.PlatUser> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                   where (user.Name == this.UserName) && user.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   && user.Password == this.Password
                   select new Models.PlatUser
                   {
                       ID = user.ID,
                       ClientID = user.ClientID,
                       Mobile = user.Mobile,
                       Email = user.Email,
                       UserName = user.Name,
                       RealName = user.RealName,
                       Password = user.Password,
                       IsMain = user.IsMain,
                       OpenID = user.OpenID
                   };
        }
    }
}
