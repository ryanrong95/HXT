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
    /// 受益人视图
    /// </summary>
    public class BeneficiariesView : UniqueView<Models.Beneficiary, CvOssReponsitory>
    {
        internal BeneficiariesView()
        {

        }
        internal BeneficiariesView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Beneficiary> GetIQueryable()
        {
            var contactsView = new ContactsView(this.Reponsitory);
            var companiesView = new CompaniesView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Beneficiaries>()
                       join _contact in contactsView on entity.ContactID equals _contact.ID into contacts
                       from contact in contacts.DefaultIfEmpty()
                       join _company in companiesView on entity.CompanyID equals _company.ID into companies
                       from company in companies.DefaultIfEmpty()
                       select new Models.Beneficiary
                       {
                           ID = entity.ID,
                           Bank = entity.Bank,
                           Methord = (Models.Methord)entity.Methord,
                           Currency = (Needs.Underly.Currency)entity.Currency,

                           Address = entity.Address,

                           Account = entity.Account,
                           SwiftCode = entity.SwiftCode,
                           Contact = contact,

                           Company = company,

                       };

            return linq;
        }

    }
}
