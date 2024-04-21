using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 畅运美元信用视图
    /// </summary>
    public class CreditsUsdStatisticsView : QueryView<CreditStatistic, PvbCrmReponsitory>
    {
        public CreditsUsdStatisticsView()
        {
        }

        public CreditsUsdStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.CreditsUsdStatisticsView<PvbCrmReponsitory>();
        }
    }
}