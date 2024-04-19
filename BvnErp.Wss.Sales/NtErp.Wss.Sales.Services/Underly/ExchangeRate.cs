using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 汇率  rate
    /// </summary>
    public class ExchangeRate
    {
        public District District { get; set; }

        public Currency From { get; set; }

        public Currency To { get; set; }

        public decimal Value { get; set; }
    }
}
