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
    /// 流水表汇总金额
    /// </summary>
    /// <remarks>根据类型、币种、payer、payee 分组 Price</remarks>
    /// <typeparam name="TReponsitory"></typeparam>
    public class FlowAccountsStatisticsView<TReponsitory> : QueryView<FlowAccountsStatistic, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public FlowAccountsStatisticsView()
        {

        }

        public FlowAccountsStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccountsStatistic> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowAccountsStatisticsView>()
                select new FlowAccountsStatistic()
                {
                    Currency = (Currency)entity.Currency,
                    Price = entity.Price,
                    Business = entity.Business,
                    Payer = entity.Payer,
                    Payee = entity.Payee,
                    Type = (AccountType)entity.Type,
                };
        }
    }
}
