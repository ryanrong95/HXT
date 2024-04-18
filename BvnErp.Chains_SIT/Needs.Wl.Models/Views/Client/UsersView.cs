using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class UsersView : View<Models.User, ScCustomsReponsitory>
    {
        public UsersView()
        {

        }

        internal UsersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.User> GetIQueryable()
        {
            return from user in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>()
                   where user.Status == (int)Enums.Status.Normal
                   select new Models.User
                   {
                       ID = user.ID,
                       OpenID = user.OpenID,
                       Name = user.Name,
                       RealName = user.RealName,
                       Password = user.Password,
                       Email = user.Email,
                       Mobile = user.Mobile,
                       AdminID = user.AdminID,
                       IsMain = user.IsMain,
                       ClientID = user.ClientID,
                       Status = user.Status,
                       CreateDate = user.CreateDate,
                       UpdateDate = user.UpdateDate,
                       Summary = user.Summary
                   };
        }
    }
}
