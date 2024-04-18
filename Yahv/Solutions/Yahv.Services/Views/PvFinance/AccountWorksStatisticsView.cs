using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models.PvFinance;
using Yahv.Underly;

namespace Yahv.Services.Views.PvFinance
{
    /// <summary>
    /// 认领收款 余额视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class AccountWorksStatisticsView<TReponsitory> : QueryView<AccountWorksStatistic, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public AccountWorksStatisticsView()
        {
        }

        public AccountWorksStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AccountWorksStatistic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AccountWorksStatisticsView>()
                   where entity.ClaimantID != null
                   select new AccountWorksStatistic()
                   {
                       LeftPrice = entity.LeftPrice,
                       RightPrice = entity.RightPrice,
                       ClaimantID = entity.ClaimantID,
                       Company = entity.Company,
                       Currency = (Currency)entity.Currency,
                       FormCode = entity.FormCode,
                       PayeeLeftID = entity.PayeeLeftID,
                   };
        }
    }
}