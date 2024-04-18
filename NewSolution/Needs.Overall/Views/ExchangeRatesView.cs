using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Overall.Models;
using Needs.Underly;
using System.Linq;

namespace Needs.Overall.Views
{
    class ExchangeRatesView : QueryView<ExchangeRate, BvOverallsReponsitory>
    {
        public ExchangeRatesView()
        {

        }

        protected override IQueryable<ExchangeRate> GetIQueryable()
        {
            return from rate in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.ExchangeRates>()
                   select new ExchangeRate
                   {
                       Type = rate.Type,
                       District = (District)rate.District,
                       From = (Currency)rate.From,
                       To = (Currency)rate.To,
                       Value = rate.Value
                   };
        }
    }
}
