using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderControlItems : BaseItems<OrderControlItem>
    {
        internal OrderControlItems(IEnumerable<OrderControlItem> enums) : base(enums)
        {
        }

        internal OrderControlItems(IEnumerable<OrderControlItem> enums, Action<OrderControlItem> action) : base(enums, action)
        {
        }

        public override void Add(OrderControlItem item)
        {
            base.Add(item);
        }
    }
}
