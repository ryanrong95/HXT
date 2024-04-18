using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public abstract class BaseCalculate
    {
        public List<string> PackNos { get; set; }

        public BaseCalculate(List<string> packNos)
        {
            this.PackNos = packNos;
        }

        public abstract int Calculate();
    }
}
