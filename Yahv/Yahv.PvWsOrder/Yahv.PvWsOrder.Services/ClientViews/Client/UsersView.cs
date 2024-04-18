using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;

namespace Yahv.PvWsOrder.Services.ClientViews.Client
{
    public class UsersView : UniqueView<User, ScCustomReponsitory>
    {
        protected override IQueryable<User> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Users>()
                   select new User
                   {
                       ID = user.ID,
                       UserName = user.Name,
                       Mobile = user.Mobile,
                       Status = (Enums.UserStatus)user.Status,
                   };
        }

        /// <summary>
        /// 将已经存在的用户名检查出来
        /// </summary>
        public List<string> CheckExistUserNames(List<string> usernames)
        {
            var exists = this.IQueryable.Where(t => usernames.Contains(t.UserName))
                .Select(item => item.UserName).ToList();
            return exists;
        }

    }
}
