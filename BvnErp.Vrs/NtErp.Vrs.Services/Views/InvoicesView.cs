using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Vrs.Services.Models;
using Layer.Data.Sqls;
using NtErp.Vrs.Services.Enums;

namespace NtErp.Vrs.Services.Views
{
    public class InvoicesView : Needs.Linq.UniqueView<Models.Invoice, BvnVrsReponsitory>
    {
        public InvoicesView()
        {
        }
        internal InvoicesView(BvnVrsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            var companyview = new CompaniesView(this.Reponsitory);
            var contactview = new ContactsView(this.Reponsitory);

            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Invoices>()
                        join company in companyview on entity.CompanyID equals company.ID
                        join contact in contactview on entity.ContactID equals contact.ID
                        select new Models.Invoice()
                        {
                            ID = entity.ID,
                            Required=entity.Required,
                            Type = (InvoiceType)entity.Type,
                            CompanyID = entity.CompanyID,
                            ContactID = entity.ContactID,
                            Address = entity.Address,
                            Postzip = entity.Postzip,
                            Bank = entity.Bank,
                            BankAddress = entity.BankAddress,
                            Account = entity.Account,
                            SwiftCode = entity.SwiftCode,
                            Company = company,
                            Contact = contact
                        };
            return linqs;
        }
    }
}
