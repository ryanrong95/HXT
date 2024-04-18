using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Services.Models;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
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
