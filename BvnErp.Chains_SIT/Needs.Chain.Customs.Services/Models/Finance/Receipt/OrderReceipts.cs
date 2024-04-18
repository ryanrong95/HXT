using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderReceipts : BaseItems<OrderReceipt>
    {
        internal OrderReceipts(IEnumerable<OrderReceipt> enums) : base(enums)
        {
        }

        internal OrderReceipts(IEnumerable<OrderReceipt> enums, Action<OrderReceipt> action) : base(enums, action)
        {
        }

        public override void Add(OrderReceipt item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }

    public class OrderReceivables : BaseItems<OrderReceivable>
    {
        internal OrderReceivables(IEnumerable<OrderReceivable> enums) : base(enums)
        {
        }

        internal OrderReceivables(IEnumerable<OrderReceivable> enums, Action<OrderReceivable> action) : base(enums, action)
        {
        }

        public override void Add(OrderReceivable item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }

    public class OrderReceiveds : BaseItems<OrderReceived>
    {
        internal OrderReceiveds(IEnumerable<OrderReceived> enums) : base(enums)
        {
        }

        internal OrderReceiveds(IEnumerable<OrderReceived> enums, Action<OrderReceived> action) : base(enums, action)
        {
        }

        public override void Add(OrderReceived item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
