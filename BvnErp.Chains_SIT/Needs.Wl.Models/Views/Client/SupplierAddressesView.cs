using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class SupplierAddressesView : View<Models.ClientSupplierAddress, ScCustomsReponsitory>
    {
        private string SupplierID;

        public SupplierAddressesView(string supplierID)
        {
            this.SupplierID = supplierID;
        }

        internal SupplierAddressesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientSupplierAddress> GetIQueryable()
        {
            return from address in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>()
                   join contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>() on address.ContactID equals contact.ID
                   where address.ClientSupplierID == this.SupplierID && address.Status == (int)Enums.Status.Normal
                   orderby address.IsDefault descending
                   select new Models.ClientSupplierAddress
                   {
                       ID = address.ID,
                       ClientSupplierID = address.ClientSupplierID,
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
                           Status = (Needs.Wl.Models.Enums.Status)contact.Status,
                           CreateDate = contact.CreateDate
                       },
                       Address = address.Address,
                       ZipCode = address.ZipCode,
                       IsDefault = address.IsDefault,
                       Status = address.Status,
                       CreateDate = address.CreateDate,
                       UpdateDate = address.UpdateDate,
                       Summary = address.Summary
                   };
        }

        public Models.ClientSupplierAddress GetDefault()
        {
            return this.GetIQueryable().Where(s => s.IsDefault).FirstOrDefault();
        }
    }
}
