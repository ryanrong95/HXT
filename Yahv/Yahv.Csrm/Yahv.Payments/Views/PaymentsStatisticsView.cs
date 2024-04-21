using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 应付实付 视图列表
    /// </summary>
    public class PaymentsStatisticsView : QueryView<PaymentsStatistic, PvbCrmReponsitory>
    {
        public PaymentsStatisticsView()
        {

        }

        public PaymentsStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<PaymentsStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.PaymentsStatisticsView<PvbCrmReponsitory>();
        }
    }
}
