using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderItems : BaseItems<OrderItem>
    {
        internal OrderItems(IEnumerable<OrderItem> enums) : base(enums)
        {
        }

        internal OrderItems(IEnumerable<OrderItem> enums, Action<OrderItem> action) : base(enums, action)
        {
        }

        public override void Add(OrderItem item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
