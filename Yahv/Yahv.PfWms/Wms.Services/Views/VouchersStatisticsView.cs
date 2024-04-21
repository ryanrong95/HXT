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
    /// 对账单视图
    /// </summary>
    public class VouchersStatisticsView : QueryView<VoucherStatistic, PvWmsRepository>
    {
        public VouchersStatisticsView()
        {

        }

        public VouchersStatisticsView(PvWmsRepository repository) : base(repository)
        {

        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return new Yahv.Services.Views.VouchersStatisticsView<PvWmsRepository>();
        }
    }
}
