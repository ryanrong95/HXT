using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class UsersToken : UsersView
    {

        protected override IQueryable<Models.User> GetIQueryable()
        {
            return from token in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UserTokens>()
                   join user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on token.UserID equals user.ID
                   select new Models.User
                   {
                       ID = token.UserID,
                       IP=token.IP,
                       Token=token.Token,
                       Name = user.Name,
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