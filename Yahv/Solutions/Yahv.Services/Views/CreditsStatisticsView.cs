using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 信用批复统计
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CreditsStatisticsView<TReponsitory> : QueryView<CreditStatistic, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CreditsStatisticsView()
        {

        }

        public CreditsStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CreditsStatisticsView>()
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
