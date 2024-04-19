using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    public class TaxRate
    {
        public Currency Currency { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
