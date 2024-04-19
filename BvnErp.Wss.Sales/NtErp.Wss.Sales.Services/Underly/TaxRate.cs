using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    public class TaxRate
    {
        public Currency Currency { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
