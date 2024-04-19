using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    sealed class DValue : Elements
    {
        public DValue(object value) : base(value)
        {

        }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return $"{this.GetType().FullName}[{this.Count}]";
            }

            return this.Value.ToString();
        }
    }
}
