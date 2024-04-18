using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class CreditsUsdStatisticsView<TReponsitory> : QueryView<CreditStatistic, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CreditsUsdStatisticsView()
        {

        }

        public CreditsUsdStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CreditsUsdStatisticsView>()
                   select new CreditStatistic()
                   {
                       Currency = (Currency)entity.Currency,
                       Catalog = entity.Catalog,
                       Business = entity.Business,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       Cost = entity.Cost,
                       Total = entity.Total,
                   };
        }
    }
}