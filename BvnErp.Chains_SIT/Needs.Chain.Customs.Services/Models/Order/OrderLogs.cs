using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderLogs : BaseItems<OrderLog>
    {
        internal OrderLogs(IEnumerable<OrderLog> enums) : base(enums)
        {
        }

        internal OrderLogs(IEnumerable<OrderLog> enums, Action<OrderLog> action) : base(enums, action)
        {
        }

        public override void Add(OrderLog item)
        {
            base.Add(item);
        }
    }
}
