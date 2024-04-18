using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderItemTaxes : BaseItems<OrderItemTax>
    {
        internal OrderItemTaxes(IEnumerable<OrderItemTax> enums): base(enums)
        {
        }

        internal OrderItemTaxes(IEnumerable<OrderItemTax> enums, Action<OrderItemTax> action) : base(enums, action)
        {
        }

        public override void Add(OrderItemTax item)
        {
            base.Add(item);
        }
    }
}
