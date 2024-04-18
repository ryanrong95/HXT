using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class UsersView : UniqueView<Models.User, ScCustomsReponsitory>
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
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on  user.ClientID equals client.ID
                   where user.Status == (int)Enums.Status.Normal
                   select new Models.User
                   {
                       ID = user.ID,
                       OpenID=user.OpenID,
                       Name = user.Name,
                       RealName=user.RealName,
                       Password = user.Password,
                       Email = user.Email,
                       Mobile = user.Mobile,
                       AdminID = user.AdminID,
                       IsMain = user.IsMain,
                       ClientID = user.ClientID,
                       Status = (Enums.Status)user.Status,
                       CreateDate = user.CreateDate,
                       UpdateDate = user.UpdateDate,
                       Summary = user.Summary
                   };
        }
    }

    /// <summary>
    /// 客户的会员
    /// </summary>
    public sealed class ClientUsersView : UsersView
    {
        Models.Client Client;

        public ClientUsersView(Models.Client client)
        {
            this.Client = client;
        }

        protected override IQueryable<Models.User> GetIQueryable()
        {
            return from user in base.GetIQueryable()
                   where user.ClientID == this.Client.ID && user.Status != Enums.Status.Delete
                   select user;
        }
    }
}
