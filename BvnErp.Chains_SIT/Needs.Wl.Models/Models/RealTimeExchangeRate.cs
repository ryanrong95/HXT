using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 实时汇率
    /// </summary>
    public sealed class RealTimeExchangeRate : ExchangeRate
    {
        public RealTimeExchangeRate() : base(Enums.ExchangeRateType.RealTime)
        {

        }
    }
}
