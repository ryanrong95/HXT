using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CompaniesView : UniqueView<Models.Company, ScCustomsReponsitory>
    {
        public CompaniesView()
        {

        }

        internal CompaniesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Company> GetIQueryable()
        {
            var contactView = new ContactsView(this.Reponsitory);
            return from company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>()
                   join contact in contactView on company.ContactID equals contact.ID
                   select new Models.Company
                   {
                       ID = company.ID,
                       Name = company.Name,
                       Code = company.Code,
                       CustomsCode = company.CustomsCode,
                       Corporate = company.Corporate,
                       Address = company.Address,
                       Status = (Enums.Status)company.Status,
                       Contact = contact,
                       CreateDate = company.CreateDate,
                       UpdateDate = company.UpdateDate,
                       Summary = company.Summary,
                   };
        }
    }
}