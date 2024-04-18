using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DeliveryOrderItems : BaseItems<DeliveryOrderItem>
    {
        internal DeliveryOrderItems(IEnumerable<DeliveryOrderItem> enums) : base(enums)
        {
        }

        internal DeliveryOrderItems(IEnumerable<DeliveryOrderItem> enums, Action<DeliveryOrderItem> action) : base(enums, action)
        {
        }

        public override void Add(DeliveryOrderItem item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
