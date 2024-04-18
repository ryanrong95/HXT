using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    public class OrderPayExchangeSuppliers : BaseItems<OrderPayExchangeSupplier>
    {
        public OrderPayExchangeSuppliers(IEnumerable<OrderPayExchangeSupplier> enums) : base(enums)
        {

        }

        public override void Remove(OrderPayExchangeSupplier item)
        {
            base.Remove(item);
        }

        public override void Add(OrderPayExchangeSupplier item)
        {
            base.Add(item);
        }
    }
}