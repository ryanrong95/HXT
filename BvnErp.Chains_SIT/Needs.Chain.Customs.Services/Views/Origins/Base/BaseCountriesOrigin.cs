using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class BaseCountriesOrigin : UniqueView<Models.Country, ScCustomsReponsitory>
    {
        public BaseCountriesOrigin()
        {
        }

        public BaseCountriesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Country> GetIQueryable()
        {
            return from country in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>()
                   select new Models.Country
                   {
                       ID = country.ID,
                       Code = country.Code,
                       Name = country.Name,
                       EnglishName = country.EnglishName,
                       EditionOneCode = country.EditionOne,
                       TypeInt = country.Type,
                   };
        }
    }
}
