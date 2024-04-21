using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models
{
    public class ExchangeRate
    {
        public string Type { get; set; }
        public ExchangeDistrict District { get; set; }
        public Currency From { get; set; }
        public Currency To { get; set; }
        public decimal Value { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
