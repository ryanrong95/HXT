using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientConsigneesView : UniqueView<Models.ClientConsignee, ScCustomsReponsitory>
    {
        public ClientConsigneesView()
        {

        }
        internal ClientConsigneesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientConsignee> GetIQueryable()
        {
            var contactView = new ContactsView(this.Reponsitory);
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientConsignees>()
                   join contact in contactView on consignee.ContactID equals contact.ID
                   where consignee.Status == (int)Enums.Status.Normal
                   orderby consignee.CreateDate descending
                   select new Models.ClientConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name=consignee.Name,
                       Address = consignee.Address,
                       Contact = contact,
                       IsDefault = consignee.IsDefault,
                       Status = (Enums.Status)consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }
    }
}
