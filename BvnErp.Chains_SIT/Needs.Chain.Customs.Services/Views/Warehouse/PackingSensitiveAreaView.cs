using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PackingSensitiveAreaView : UniqueView<Models.Country, ScCustomsReponsitory>
    {
        public PackingSensitiveAreaView()
        {
        }

        internal PackingSensitiveAreaView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Country> GetIQueryable()
        {
            return from sensitive in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingSensitiveAreas>()
                   join country in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>() on sensitive.Code equals country.Code
                   where sensitive.Status == (int)Enums.Status.Normal
                   select new Models.Country
                   {
                       ID = country.ID,
                       Code = country.Code,
                       Name = country.Name,
                       EnglishName = country.EnglishName,
                       EditionOneCode = country.EditionOne,
                   };
        }
    }
}
