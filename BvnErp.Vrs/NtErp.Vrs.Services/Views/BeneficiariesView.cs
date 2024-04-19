using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
    public class BeneficiariesView : Needs.Linq.UniqueView<Models.Beneficiary, Layer.Data.Sqls.BvnVrsReponsitory>
    {
        public BeneficiariesView()
        {
        }
        internal BeneficiariesView(BvnVrsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Beneficiary> GetIQueryable()
        {
            ContactsView contactview = new ContactsView(this.Reponsitory);
            CompaniesView companiesView = new CompaniesView(this.Reponsitory);
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Beneficiaries>()
                   join contact in contactview on entity.ContactID equals contact.ID
                   join company in companiesView on entity.CompanyID equals company.ID
                   select new Models.Beneficiary
                   {
                       ID = entity.ID,
                       Bank = entity.Bank,
                       Method = (Enums.PayMethod)entity.Method,
                       Currency = (Needs.Underly.Currency)entity.Currency,
                       Address = entity.Address,
                       SwiftCode = entity.SwiftCode,
                       ContactID = entity.ContactID,
                       CompanyID=entity.CompanyID,                      
                       Status = (Enums.Status)entity.Status,
                       Contact = contact,
                       Company = company
                   };
        }
    }
}
