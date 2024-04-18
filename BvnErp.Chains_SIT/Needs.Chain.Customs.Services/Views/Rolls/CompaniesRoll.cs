using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class CompaniesRoll : UniqueView<Models.Company, ScCustomsReponsitory>
    {
        internal CompaniesRoll()
        {

        }

        internal CompaniesRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Company> GetIQueryable()
        {
            var companiesView = new Origins.CompaniesOrigin(this.Reponsitory);
            var contactsView = new Origins.ContactsOrigin(this.Reponsitory);
            return from company in companiesView
                   join contact in contactsView on company.ContactID equals contact.ID
                   select new Models.Company
                   {
                       ID = company.ID,
                       Name = company.Name,
                       Code = company.Code,
                       CustomsCode = company.CustomsCode,
                       Corporate = company.Corporate,
                       Address = company.Address,
                       Status = company.Status,
                       Contact = contact,
                       CreateDate = company.CreateDate,
                       UpdateDate = company.UpdateDate,
                       Summary = company.Summary,
                   };
        }
    }
}
