using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
    public class CompaniesView : UniqueView<Models.Company, BvnVrsReponsitory>
    {
        public CompaniesView()
        {
        }
        internal CompaniesView(BvnVrsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Company> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Companies>()
                   select new Models.Company()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (Enums.ComapnyType)entity.Type,
                       Code = entity.Code,
                       Address = entity.Address,
                       RegisteredCapital = entity.RegisteredCapital,
                       CorporateRepresentative = entity.CorporateRepresentative,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
            return linqs;
        }
    }
}
