using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    /// <summary>
    /// 支出账单
    /// </summary>
    public class PaymentsStatisticsView : QueryView<PaymentsStatistic, PvWmsRepository>
    {
        public PaymentsStatisticsView()
        {

        }

        public PaymentsStatisticsView(PvWmsRepository repository) : base(repository)
        {

        }

        protected override IQueryable<PaymentsStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.PaymentsStatisticsView<PvWmsRepository>();
        }
    }
}
