using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 应收实收统计视图
    /// </summary>
    public class VouchersCnyStatisticsView : QueryView<VoucherCnyStatistic, PvbCrmReponsitory>
    {
        public VouchersCnyStatisticsView()
        {

        }

        public VouchersCnyStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<VoucherCnyStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.VouchersCnyStatisticsView<PvbCrmReponsitory>();
        }
    }
}