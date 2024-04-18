using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class CompaniesOrigin : UniqueView<Models.Company, ScCustomsReponsitory>
    {
        internal CompaniesOrigin()
        {
        }

        internal CompaniesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Company> GetIQueryable()
        {
            return from company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>()
                   select new Models.Company
                   {
                       ID = company.ID,
                       Name = company.Name,
                       Code = company.Code,
                       CustomsCode = company.CustomsCode,
                       Corporate = company.Corporate,
                       Address = company.Address,
                       Status = (Enums.Status)company.Status,
                       ContactID = company.ContactID,
                       CreateDate = company.CreateDate,
                       UpdateDate = company.UpdateDate,
                       Summary = company.Summary,
                   };
        }
    }
}
