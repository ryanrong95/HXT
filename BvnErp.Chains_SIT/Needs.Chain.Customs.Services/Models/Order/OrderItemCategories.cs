using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderItemCategories : BaseItems<OrderItemCategory>
    {
        internal OrderItemCategories(IEnumerable<OrderItemCategory> enums) : base(enums)
        {
        }

        internal OrderItemCategories(IEnumerable<OrderItemCategory> enums, Action<OrderItemCategory> action) : base(enums, action)
        {
        }

        public override void Add(OrderItemCategory item)
        {
            base.Add(item);
        }
    }
}
