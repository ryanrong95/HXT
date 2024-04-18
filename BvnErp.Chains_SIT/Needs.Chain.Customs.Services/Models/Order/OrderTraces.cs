using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderTraces : BaseItems<OrderTrace>
    {
        internal OrderTraces(IEnumerable<OrderTrace> enums): base(enums)
        {
        }

        internal OrderTraces(IEnumerable<OrderTrace> enums, Action<OrderTrace> action) : base(enums, action)
        {
        }

        public override void Add(OrderTrace item)
        {
            base.Add(item);
        }
    }
}
