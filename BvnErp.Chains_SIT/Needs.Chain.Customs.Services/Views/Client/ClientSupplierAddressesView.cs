using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientSupplierAddressesView : UniqueView<Models.ClientSupplierAddress, ScCustomsReponsitory>
    {
        public ClientSupplierAddressesView()
        {
        }

        internal ClientSupplierAddressesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientSupplierAddress> GetIQueryable()
        {
            var contactsView = new ContactsView(this.Reponsitory);

            return from address in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>()
                   join contact in contactsView on address.ContactID equals contact.ID
                   orderby address.CreateDate descending
                   select new Models.ClientSupplierAddress
                   {
                       ID = address.ID,
                       ClientSupplierID = address.ClientSupplierID,
                       Contact = contact,
                       Address = address.Address,
                       ZipCode = address.ZipCode,
                       IsDefault = address.IsDefault,
                       Status = (Enums.Status)address.Status,
                       CreateDate = address.CreateDate,
                       UpdateDate = address.UpdateDate,
                       Summary = address.Summary,
                       Place=address.Place
                   };
        }
    }
}
