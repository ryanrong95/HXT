using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   
    public abstract class StockingStrategy
    {
        public abstract int CalculateDays();
    }
}
