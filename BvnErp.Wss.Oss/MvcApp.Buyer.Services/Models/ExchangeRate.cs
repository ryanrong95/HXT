using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    /// <summary>
    /// 汇率  rate
    /// </summary>
    public class ExchangeRate
    {
        public Needs.Underly.District District { get; set; }

        public Currency From { get; set; }

        public Currency To { get; set; }

        public decimal Value { get; set; }
    }
}
