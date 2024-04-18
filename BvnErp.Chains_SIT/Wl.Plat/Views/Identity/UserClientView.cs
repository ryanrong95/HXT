using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 当前登录用户的客户主体信息
    /// </summary>
    public class UserClientView : Needs.Wl.Models.Views.ClientsView
    {
        IPlatUser User;

        public UserClientView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Models.Client> GetIQueryable()
        {
            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                   join contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>() on company.ContactID equals contact.ID
                   join clientAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientMerchandiserAdminView>() on client.ID equals clientAdmin.ID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSaleAdminView>() on client.ID equals saleAdmin.ID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()
                   orderby client.CreateDate descending
                   where client.ID == this.User.ClientID
                   && client.ClientStatus == (int)Needs.Wl.Models.Enums.ClientStatus.Confirmed
                   && client.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Needs.Wl.Models.Client
                   {
                       ID = client.ID,
                       Company = new Needs.Wl.Models.Company()
                       {
                           ID = company.ID,
                           Name = company.Name,
                           Address = company.Address,
                           Code = company.Code,
                           Corporate = company.Corporate,
                           Contact = new Needs.Wl.Models.Contact()
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
                       ClientType = (Needs.Wl.Models.Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Needs.Wl.Models.Enums.ClientRank)client.ClientRank,
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
                       CreateDate = client.CreateDate,
                       Summary = client.Summary,
                       IsValid = client.IsValid
                   };
        }
    }
}