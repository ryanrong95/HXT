using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 发票视图
    /// </summary>
    public class InvoicesView : UniqueView<Models.Invoice, CvOssReponsitory>
    {
        internal InvoicesView()
        {

        }
        internal InvoicesView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Invoice> GetIQueryable()
        {
            var companiesView = new CompaniesView(this.Reponsitory);
            var contactsView = new ContactsView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Invoices>()
                       join _company in companiesView on entity.CompanyID equals _company.ID into companies
                       from company in companies.DefaultIfEmpty()
                       join _contact in contactsView on entity.ContactID equals _contact.ID into contacts
                       from contact in contacts.DefaultIfEmpty()
                       select new Models.Invoice
                       {
                           ID = entity.ID,
                           Required = entity.Required,
                           Type = (Models.InvoiceType)entity.Type,
                           CompanyID = entity.CompanyID,
                           ContactID = entity.ContactID,
                           Address = entity.Address,
                           Postzip = entity.Postzip,
                           Bank = entity.Bank,
                           BankAddress = entity.BankAddress,
                           Account = entity.Account,
                           SwiftCode = entity.SwiftCode,

                           Contact = contact,
                           Company = company
                       };

            return linq;
        }

    }
}
