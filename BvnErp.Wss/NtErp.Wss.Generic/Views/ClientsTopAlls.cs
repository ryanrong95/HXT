using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using System.Linq;
using NtErp.Services.Views;

namespace NtErp.Wss.Services.Generic.Views
{
    /// <summary>
    /// Client
    /// </summary>
    public class ClientsTopAlls : UniqueFiter<Models.ClientTop, ClientsTopView<BvnErpReponsitory>>
    {
        internal ClientsTopAlls() : base(new ClientsTopView<BvnErpReponsitory>())
        {

        }

        protected override IQueryable<Models.ClientTop> GetIQueryable()
        {
            return from admin in this.View
                   select new Models.ClientTop
                   {
                       Email = admin.Email,
                       ID = admin.ID,
                       Mobile = admin.Mobile,
                       UserName = admin.UserName,
                       Status = admin.Status,
                       
                   };
        }
    }
}
