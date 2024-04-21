using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Rolls;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 应收实收统计视图
    /// </summary>
    public class VouchersStatisticsView : QueryView<VoucherStatistic, PvbCrmReponsitory>
    {
        public VouchersStatisticsView()
        {

        }

        public VouchersStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.VouchersStatisticsView<PvbCrmReponsitory>();
        }
    }
}