using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderPayExchangeSuppliers : BaseItems<OrderPayExchangeSupplier>
    {
        internal OrderPayExchangeSuppliers(IEnumerable<OrderPayExchangeSupplier> enums) : base(enums)
        {
        }

        internal OrderPayExchangeSuppliers(IEnumerable<OrderPayExchangeSupplier> enums, Action<OrderPayExchangeSupplier> action) : base(enums, action)
        {
        }

        public override void Add(OrderPayExchangeSupplier item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
