using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 实时汇率的视图
    /// </summary>
    public class RealTimeExchangeRatesView : ExchangeRatesView<RealTimeExchangeRate>
    {
        public RealTimeExchangeRatesView() : base(ExchangeRateType.RealTime)
        {

        }
    }

    /// <summary>
    /// 实时汇率
    /// </summary>
    public sealed class RealTimeExchangeRate : ExchangeRate
    {
        public RealTimeExchangeRate() : base(ExchangeRateType.RealTime)
        {

        }
    }
}
