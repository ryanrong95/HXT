using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Services.Views;
using System.Linq;

namespace NtErp.Wss.Services.Generic.Views
{
    /// <summary>
    /// Client
    /// </summary>
    public class ClientsTopView : UniqueFiter<Models.ClientTop, MyClientsTopView<BvnErpReponsitory>>
    {
        IGenericAdmin admin;
        internal ClientsTopView(IGenericAdmin admin) : base(new MyClientsTopView<BvnErpReponsitory>(admin))
        {
            this.admin = admin;
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
                       CreateDate = admin.CreateDate
                   };
        }

        public void Bind(string clientid,string adminid)
        {
            using (var repository = new BvnErpReponsitory())
            {
                //删除 全部的item.ClientID == clientid

                repository.Delete<Layer.Data.Sqls.BvnErp.MapsAdminClient>(item => item.ClientID == clientid);
                repository.Insert(new Layer.Data.Sqls.BvnErp.MapsAdminClient
                {
                    AdminID = adminid,
                    ClientID = clientid,
                    Type = ""
                });
            }
        }

        public void UnBind(string clientid,string adminid)
        {
            using (var repository = new BvnErpReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnErp.MapsAdminClient>(item => item.AdminID == adminid && item.ClientID == clientid);
            }
        }
    }
}
