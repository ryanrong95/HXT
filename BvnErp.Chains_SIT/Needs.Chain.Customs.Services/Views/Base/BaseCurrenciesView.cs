using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseCurrenciesView : UniqueView<Models.Currency, ScCustomsReponsitory>
    {
        public BaseCurrenciesView()
        {
        }

        internal BaseCurrenciesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Currency> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCurrencies>()
                   select new Models.Currency
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name,
                       EnglishName = entity.EnglishName
                   };
        }
    }
}