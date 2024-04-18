using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class UsersAlls : UniqueView<Models.PlatUser, ScCustomsReponsitory>
    {
        public UsersAlls()
        {

        }

        protected override IQueryable<Models.PlatUser> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                   select new Models.PlatUser
                   {
                       ID = user.ID,
                       RealName = user.RealName,
                       UserName = user.Name,
                       Password = user.Password,
                       ClientID = user.ClientID,
                       Mobile = user.Mobile,
                       IsMain = user.IsMain,
                       Email = user.Email,
                   };
        }
    }
}
