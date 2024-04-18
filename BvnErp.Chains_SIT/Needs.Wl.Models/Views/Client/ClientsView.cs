using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientsView : View<Needs.Wl.Models.Client, ScCustomsReponsitory>
    {
        public ClientsView()
        {
        }

        public ClientsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Wl.Models.Client> GetIQueryable()
        {
            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   join contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>() on company.ContactID equals contact.ID
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on client.AdminID equals admin.ID
                   join clientAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientMerchandiserAdminView>() on client.ID equals clientAdmin.ID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSaleAdminView>() on client.ID equals saleAdmin.ID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()
                   orderby client.CreateDate descending
                   select new Needs.Wl.Models.Client
                   {
                       ID = client.ID,
                       Company = new Company()
                       {
                           ID = company.ID,
                           Name = company.Name,
                           Address = company.Address,
                           Code = company.Code,
                           Corporate = company.Corporate,
                           Contact = new Contact()
                           {
                               ID = contact.ID,
                               Name = contact.Name,
                               Mobile = contact.Mobile,
                               Email = contact.Email,
                               Fax = contact.Fax,
                               Tel = contact.Tel,
                               QQ = contact.QQ,
                           },
                           CreateDate = company.CreateDate,
                           CustomsCode = company.CustomsCode,
                           Summary = company.Summary
                       },
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,                   
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       Merchandiser = new Needs.Wl.Models.Admin
                       {
                           ID = cadmin.ID,
                           RealName = cadmin.RealName,
                           Tel = cadmin.Tel,
                           Email = cadmin.Email,
                           Mobile = cadmin.Mobile
                       },
                       ServiceManager = new Needs.Wl.Models.Admin
                       {
                           ID = sale.ID,
                           RealName = sale.RealName,
                           Tel = sale.Tel,
                           Email = sale.Email,
                           Mobile = sale.Mobile
                       },
                       Admin = new Admin()
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName
                       },
                       Status = client.Status,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary
                   };
        }
    }
}