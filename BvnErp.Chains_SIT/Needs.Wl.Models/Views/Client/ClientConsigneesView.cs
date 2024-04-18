using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models;
using Needs.Wl.Models.Enums;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientConsigneesView : View<Models.ClientConsignee, ScCustomsReponsitory>
    {
        public ClientConsigneesView()
        {

        }
        internal ClientConsigneesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientConsignees>()
                   join contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>() on consignee.ContactID equals contact.ID
                   where consignee.Status == (int)Status.Normal
                   orderby consignee.IsDefault ascending
                   orderby consignee.CreateDate descending
                   select new Models.ClientConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name = consignee.Name,
                       Address = consignee.Address,
                       Contact = new Contact()
                       {
                           ID = contact.ID,
                           Name = contact.Name,
                           Email = contact.Email,
                           Fax = contact.Fax,
                           Mobile = contact.Mobile,
                           Tel = contact.Tel,
                           QQ = contact.QQ,
                           Summary = contact.Summary,
                           Status = (Status)contact.Status,
                           CreateDate = contact.CreateDate
                       },
                       IsDefault = consignee.IsDefault,
                       Status = consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }
    }
}