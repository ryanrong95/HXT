using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 海关汇率
    /// </summary>
    public sealed class CustomExchangeRate : ExchangeRate
    {
        public CustomExchangeRate() : base(Enums.ExchangeRateType.Custom)
        {

        }
    }
}
