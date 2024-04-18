using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 实时汇率的视图
    /// </summary>
    public class RealTimeExchangeRatesView : ExchangeRatesView<Models.RealTimeExchangeRate>
    {
        public RealTimeExchangeRatesView():base(Enums.ExchangeRateType.RealTime)
        {

        }
    }
}
