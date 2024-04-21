using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 流水记录汇总
    /// </summary>
    public class FlowAccountsStatisticsView : QueryView<FlowAccountsStatistic, PvbCrmReponsitory>
    {
        public FlowAccountsStatisticsView()
        {

        }

        public FlowAccountsStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccountsStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.FlowAccountsStatisticsView<PvbCrmReponsitory>();
        }
    }
}
