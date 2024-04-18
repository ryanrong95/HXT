using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class UsersLogin : UsersView
    {
        private string UserName;
        private string Password;

        public UsersLogin(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        protected override IQueryable<Models.User> GetIQueryable()
        {
            return from user in base.GetIQueryable()
                   where (user.Name == this.UserName) && user.Status == Enums.Status.Normal
                   && user.Password == this.Password
                   select new Models.User
                   {
                       ID = user.ID,
                       Name = user.Name,
                       OpenID=user.OpenID,
                       Email = user.Email,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       ClientID = user.ClientID,
                       Password = user.Password,
                       CreateDate = user.CreateDate,
                       UpdateDate = user.UpdateDate,
                       Status = (Enums.Status)user.Status,
                       Summary = user.Summary
                   };
        }
    }
}
