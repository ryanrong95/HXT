using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views.Origins;
using Yahv.Services.Models;

namespace Yahv.Payments.Views.Rolls
{
    /// <summary>
    /// 信用视图统计
    /// </summary>
    public class CreditsStatisticsView : QueryView<CreditStatistic, PvbCrmReponsitory>
    {
        public CreditsStatisticsView()
        {

        }

        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.CreditsStatisticsView<PvbCrmReponsitory>();
        }
    }
}
