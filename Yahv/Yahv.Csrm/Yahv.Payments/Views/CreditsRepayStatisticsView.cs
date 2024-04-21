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
    public class CreditsRepayStatisticsView : QueryView<CreditsStatistic, PvbCrmReponsitory>
    {
        public CreditsRepayStatisticsView()
        {

        }

        public CreditsRepayStatisticsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<CreditsStatistic> GetIQueryable()
        {
            return null;
            //return new Yahv.Services.Views.CreditsRepayStatisticsView<PvbCrmReponsitory>();
        }
    }
}
